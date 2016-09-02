﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using EPMLiveCore.WorkEngineService;

namespace EPMLiveCore.Infrastructure
{
    public sealed class CacheStore : IDisposable
    {
        #region Fields (7) 

        private static volatile CacheStore _instance;
        private static readonly object Locker = new Object();
        private readonly List<string> _indefiniteKeys;
        private readonly Dictionary<string, Dictionary<string, CachedValue>> _store;
        private readonly Timer _timer;
        private bool _disposed;
        private long _ticks;

        #endregion Fields 

        #region Constructors (2) 

        private CacheStore()
        {
            _store = new Dictionary<string, Dictionary<string, CachedValue>>();
            _indefiniteKeys = new List<string>();
            _ticks = DateTime.Now.Ticks;
            _timer = new Timer(Cleanup, null, 300000, 300000);
        }

        ~CacheStore()
        {
            Dispose(false);
        }

        #endregion Constructors 

        #region Properties (1) 

        public static CacheStore Current
        {
            get
            {
                if (_instance != null) return _instance;

                lock (Locker)
                {
                    if (_instance == null)
                    {
                        _instance = new CacheStore();
                    }
                }

                return _instance;
            }
        }

        #endregion Properties 

        #region Methods (11) 

        // Public Methods (6) 

        public CachedValue Get(string key, string category, Func<object> getValue,
            bool keepIndefinite = false)
        {
            string originalKey = key;
            key = BuildKey(key, category);

            if (_store.ContainsKey(category) && _store[category].ContainsKey(key)) return _store[category][key];

            Set(originalKey, getValue(), category, keepIndefinite);

            return _store[category][keepIndefinite ? originalKey : key];
        }

        public DataTable GetDataTable()
        {
            var dataTable = new DataTable();

            dataTable.Columns.Add("Key", typeof (string));
            dataTable.Columns.Add("Value", typeof (object));
            dataTable.Columns.Add("Category", typeof (string));
            dataTable.Columns.Add("CreatedAt", typeof (DateTime));
            dataTable.Columns.Add("UpdatedAt", typeof (DateTime));
            dataTable.Columns.Add("LastReadAt", typeof (DateTime));

            foreach (var p in _store.OrderBy(p => p.Key))
            {
                foreach (var pair in p.Value.OrderBy(v => v.Value.CreatedAt))
                {
                    DataRow row = dataTable.NewRow();

                    row["Key"] = pair.Key;
                    row["Value"] = pair.Value.Value;
                    row["Category"] = p.Key;
                    row["CreatedAt"] = pair.Value.CreatedAt;
                    row["UpdatedAt"] = pair.Value.UpdatedAt;
                    row["LastReadAt"] = pair.Value.LastReadAt;

                    dataTable.Rows.Add(row);
                }
            }

            return dataTable;
        }

        public void Remove(string key, string category)
        {
            key = BuildKey(key, category);

            if (!_store.ContainsKey(category)) return;
            if (!_store[category].ContainsKey(key)) return;

            lock (Locker)
            {
                _store[category].Remove(key);
            }
        }

        public void RemoveCategory(string category)
        {
            if (_store.ContainsKey(category)) _store.Remove(category);
        }

        public void RemoveSafely(string webUrl, string category, string key = null)
        {
            if (string.IsNullOrEmpty(webUrl) || string.IsNullOrEmpty(category)) return;

            var data = new XElement("ClearCache",
                new XElement("Category", new XCData(category)),
                new XElement("Key", new XCData(key ?? string.Empty)));

            using (var weApi = new WorkEngine())
            {
                weApi.Url = webUrl + (webUrl.EndsWith("/") ? string.Empty : "/") + "_vti_bin/WorkEngine.asmx";
                weApi.UseDefaultCredentials = true;
                weApi.Execute("ClearCache", data.ToString());
            }
        }

        public void Set(string key, object value, string category, bool keepIndefinite = false)
        {
            if (keepIndefinite)
            {
                string iKey = category + key;

                if (!_indefiniteKeys.Contains(iKey))
                {
                    lock (Locker)
                    {
                        _indefiniteKeys.Add(iKey);
                    }
                }
            }

            key = BuildKey(key, category);

            lock (Locker)
            {
                var cachedValue = new CachedValue(value);

                if (!_store.ContainsKey(category))
                {
                    _store.Add(category, new Dictionary<string, CachedValue> {{key, cachedValue}});
                }
                else
                {
                    if (!_store[category].ContainsKey(key))
                    {
                        _store[category].Add(key, cachedValue);
                    }
                    else
                    {
                        _store[category][key].Value = value;
                    }
                }
            }
        }

        // Private Methods (4) 

        private string BuildKey(string key, string category)
        {
            return _indefiniteKeys.Contains(category + key) ? key : key + "_" + _ticks;
        }

        private void Cleanup(object state)
        {
            long ticks = _ticks;

            lock (Locker)
            {
                _ticks = DateTime.Now.Ticks;
            }

            // Wait for 30 seconds just in-case if 
            // something is still using an old key

            Thread.Sleep(30000);

            string oldTicks = "_" + ticks;

            var keys = new Dictionary<string, List<string>>();

            foreach (var pair in _store)
            {
                foreach (var p in pair.Value)
                {
                    string key = p.Key;

                    if (!key.EndsWith(oldTicks)) continue;

                    string category = pair.Key;

                    if (!keys.ContainsKey(category))
                    {
                        keys.Add(category, new List<string>());
                    }

                    keys[category].Add(key);
                }
            }

            RemoveKeys(keys);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _timer.Dispose();
            }

            _disposed = true;
        }

        private void RemoveKeys(Dictionary<string, List<string>> keys)
        {
            lock (Locker)
            {
                foreach (var pair in keys)
                {
                    foreach (string key in pair.Value)
                    {
                        try
                        {
                            _store[pair.Key].Remove(key);
                        }
                        catch { }
                    }
                }

                List<string> categories = (from pair in _store where pair.Value.Count == 0 select pair.Key).ToList();

                foreach (string category in categories)
                {
                    try
                    {
                        _store.Remove(category);
                    }
                    catch { }
                }
            }
        }

        // Internal Methods (1) 

        internal void Clear(string data)
        {
            XDocument document = XDocument.Parse(data);

            XElement root = document.Root;
            if (root == null) return;

            XElement catElement = root.Element("Category");
            if (catElement == null) return;

            string category = catElement.Value;
            if (string.IsNullOrEmpty(category)) return;

            string key = null;

            XElement keyElement = root.Element("Key");
            if (keyElement != null)
            {
                key = keyElement.Value;
            }

            if (string.IsNullOrEmpty(key))
            {
                RemoveCategory(category);
            }
            else
            {
                Remove(key, category);
            }
        }

        #endregion Methods 

        #region Implementation of IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    public class CachedValue
    {
        #region Fields (1) 

        private object _value;

        #endregion Fields 

        #region Constructors (1) 

        public CachedValue(object value)
        {
            CreatedAt = DateTime.Now;
            Value = value;
        }

        #endregion Constructors 

        #region Properties (4) 

        public DateTime CreatedAt { get; private set; }

        public DateTime LastReadAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        public object Value
        {
            get
            {
                LastReadAt = DateTime.Now;
                return _value;
            }
            set
            {
                _value = value;
                UpdatedAt = DateTime.Now;
            }
        }

        #endregion Properties 
    }
}