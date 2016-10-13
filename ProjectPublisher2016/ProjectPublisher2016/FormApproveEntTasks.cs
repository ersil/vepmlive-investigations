using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace ProjectPublisher2016
{
	/// <summary>
	/// Summary description for FormApproveTasks.
	/// </summary>
	public class FormApproveEntTasks : System.Windows.Forms.Form
	{
		public Connect.Tasks[] taskList;
		private Microsoft.Office.Interop.MSProject.Project pj;
		public System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.ImageList imageList1;
        private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.PictureBox pictureBox3;
		private System.Windows.Forms.ColumnHeader columnHeader7;
		private System.Windows.Forms.ColumnHeader columnHeader8;

		private bool hasSelected = false;
		private System.Windows.Forms.Label label1;
		private EPMLivePublisher.UpdateTaskItem[] updateTasks;

		private Hashtable taskHash = new Hashtable();
		private Hashtable pjFields = new Hashtable();

		public EPMLivePublisher.TaskApprovalItem []approvedTasks;

		public class UUpdateTaskItem:EPMLivePublisher.UpdateTaskItem
		{
			public Microsoft.Office.Interop.MSProject.Task task;
			public int approvalstatus = 0;
			public string approvalnotes = "";
		}

		public FormApproveEntTasks(Microsoft.Office.Interop.MSProject.Project pj, EPMLivePublisher.UpdateTaskItem[] uTasks)
		{
			this.pj = pj;
			this.updateTasks = uTasks;
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormApproveEntTasks));
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader2,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader3});
            this.listView1.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.listView1.GridLines = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(4, 31);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(652, 280);
            this.listView1.SmallImageList = this.imageList1;
            this.listView1.TabIndex = 8;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.listView1.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.listView1_GiveFeedback);
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "";
            this.columnHeader5.Width = 35;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "";
            this.columnHeader6.Width = 38;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Task Name";
            this.columnHeader2.Width = 214;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Start";
            this.columnHeader7.Width = 62;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Finish";
            this.columnHeader8.Width = 61;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "% Complete";
            this.columnHeader3.Width = 75;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(504, 315);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(72, 24);
            this.button1.TabIndex = 9;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(584, 315);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(72, 24);
            this.button2.TabIndex = 10;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(4, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(648, 28);
            this.label2.TabIndex = 15;
            this.label2.Text = "The tasks shown below have been updated by your team.  Select the Approve or Reje" +
                "ct checkbox for each task, then click the OK button.";
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.button5.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.button5.Location = new System.Drawing.Point(416, 315);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(80, 24);
            this.button5.TabIndex = 16;
            this.button5.Text = "Send E-Mail";
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Visible = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.checkBox1.Location = new System.Drawing.Point(8, 47);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(12, 12);
            this.checkBox1.TabIndex = 17;
            this.checkBox1.UseVisualStyleBackColor = false;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.checkBox2.Location = new System.Drawing.Point(44, 47);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(12, 12);
            this.checkBox2.TabIndex = 19;
            this.checkBox2.UseVisualStyleBackColor = false;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(24, 47);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(12, 11);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 20;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(60, 47);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(12, 12);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox3.TabIndex = 21;
            this.pictureBox3.TabStop = false;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 315);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(392, 16);
            this.label1.TabIndex = 22;
            this.label1.Text = "label1";
            // 
            // FormApproveEntTasks
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(4, 10);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(662, 348);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listView1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormApproveEntTasks";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Approve Tasks";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormApproveEntTasks_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void FormApproveEntTasks_Load(object sender, System.EventArgs e)
		{
			FormStatus frmStatus = new FormStatus();
			frmStatus.label1.Text = "Loading Tasks...";
			frmStatus.Show();
			frmStatus.Refresh();

			foreach(Microsoft.Office.Interop.MSProject.Task t in pj.Tasks)
			{
				if(t != null)
					taskHash.Add(t.Guid.ToUpper(),t);
			}

			foreach(Microsoft.Office.Interop.MSProject.PjField val in Enum.GetValues(typeof(Microsoft.Office.Interop.MSProject.PjField)))
			{
				if(val.ToString().Substring(0,6) == "pjTask")
				{
					if(!pjFields.Contains(val.ToString()))
						pjFields.Add(val.ToString(),(int)val);
				}
			}

			int count = BuildList();

			//label1.Text = "Updating " + count.ToString() + " of " + taskList.Length.ToString();

			frmStatus.Dispose();
		}

		private int BuildList()
		{
			int counter = 0;
			ArrayList arrNewTasks = new ArrayList();
			
			checkBox1.Parent = listView1;
			checkBox2.Parent = listView1;
			pictureBox2.Parent = listView1;
			pictureBox3.Parent = listView1;

			checkBox1.Top = 3;
			checkBox2.Top = 3;
			pictureBox2.Top = 3;
			pictureBox3.Top = 3;

			foreach(EPMLivePublisher.UpdateTaskItem ti in updateTasks)
			{
				UUpdateTaskItem taskItem = new UUpdateTaskItem();
				taskItem.finishDate = ti.finishDate;
				taskItem.percentComplete = ti.percentComplete;
				taskItem.startDate = ti.startDate;
				taskItem.taskname = ti.taskname;
				taskItem.taskuid = ti.taskuid;
				taskItem.updatefields = ti.updatefields;
				taskItem.notes = ti.notes;
				taskItem.listItemId = ti.listItemId;
				try
				{
					taskItem.taskHierarchy = ti.taskHierarchy;
				}
				catch{}
				
				if(ti.taskuid != null)
					taskItem.task = (Microsoft.Office.Interop.MSProject.Task)taskHash[ti.taskuid.ToUpper()];
				else
					taskItem.task = null;

				foreach(EPMLivePublisher.UpdateFieldItem fi in taskItem.updatefields)
				{
					if(pjFields.Contains(fi.internalFieldName))
						fi.fieldId = (int)pjFields[fi.internalFieldName];
				}
				counter++;
				if(taskItem.task != null)
					addTask(taskItem);
				else
					arrNewTasks.Add(taskItem);

				if(counter > 500)
					break;
			}
			if(arrNewTasks.Count > 0)
			{
				addGroup("New Tasks",System.Drawing.Color.Green);
				foreach(UUpdateTaskItem ti in arrNewTasks)
				{
					addTask(ti);
				}
			}

			this.label1.Text = "Updating " + counter.ToString() + " of " + updateTasks.Length.ToString();
			return counter;
		}

		private void addGroup(string name, System.Drawing.Color color)
		{
			ListViewItem li = new ListViewItem(new string[]{name});
			li.Tag = "label";
			listView1.Items.Add(li);
			listView1.Refresh();

			Font f = new Font("Arial",9,FontStyle.Bold);
			Label lbl = new Label();
			lbl.Visible = true;
			lbl.Width = listView1.Width;
			lbl.BackColor = color;
			lbl.ForeColor = System.Drawing.Color.White;
				
			lbl.Text = name;
			listView1.Controls.Add(lbl);

			lbl.Tag = li.Index;
			lbl.Top = li.Bounds.Top;
			lbl.Height = li.Bounds.Height;
			lbl.Font = f;
		}

		private void addTask(UUpdateTaskItem updateItem)
		{
			//Connect.taskList[index].isValid = true;

			Font f = new Font("Arial",8,FontStyle.Underline);
			ListViewItem li = new ListViewItem(new string[]{"","",updateItem.taskname,updateItem.startDate,updateItem.finishDate,updateItem.percentComplete + "%",""});
			li.SubItems[2].Font = f;
			li.SubItems[3].ForeColor = System.Drawing.Color.Blue;
			li.Tag = updateItem;
			listView1.Items.Add(li);

			UserControl1 ctl = new UserControl1(true);
			ctl.Name = "Approve";
			ctl.Top = li.Bounds.Top;
			ctl.Left = listView1.Columns[0].Width / 2 - ctl.Width / 2;
			listView1.Controls.Add(ctl);
			ctl.Tag = li.Index;
			ctl.lItem = li;

			UserControl1 ctl2 = new UserControl1(false);
			ctl2.Name = "Approve";
			ctl2.Top = li.Bounds.Top;
			ctl2.Left = listView1.Columns[0].Width + listView1.Columns[1].Width / 2 - ctl2.Width / 2;
			listView1.Controls.Add(ctl2);
			ctl2.Tag = li.Index;
			ctl2.lItem = li;
			ctl2.otherItem = ctl;

			ctl.otherItem = ctl2;

			ApprovalLabel lbl1 = new ApprovalLabel(pj);
			listView1.Controls.Add(lbl1);
			lbl1.Tag = li.Index;
			lbl1.linkLabel1.Text = updateItem.taskname;
			lbl1.Top = li.Bounds.Top;
			lbl1.Width = listView1.Columns[2].Width - 2;
			lbl1.Left = listView1.Columns[0].Width + listView1.Columns[1].Width + 1;
			lbl1.Visible = true;
			lbl1.lItem  = li;
			
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			if(listView1.Items.Count > 0)
			{
				checkBox1.Left = listView1.Items[0].Bounds.Left + listView1.Columns[0].Width / 2 - 12;
				pictureBox2.Left = listView1.Items[0].Bounds.Left + listView1.Columns[0].Width / 2 + 3;
				checkBox1.Refresh();
				pictureBox2.Refresh();

				checkBox2.Left = listView1.Items[0].Bounds.Left + listView1.Columns[0].Width + listView1.Columns[1].Width / 2 - 12;
				pictureBox3.Left = listView1.Items[0].Bounds.Left + listView1.Columns[0].Width + listView1.Columns[1].Width / 2 + 3;
				checkBox2.Refresh();
				pictureBox3.Refresh();
			
				foreach(Object obj in listView1.Controls)
				{
					if(obj.GetType().ToString() == "Addin.ApprovalLabel")
					{
						ApprovalLabel ctl = (ApprovalLabel)obj;
						if(listView1.Items[int.Parse(ctl.Tag.ToString())].Bounds.Top < listView1.Items[int.Parse(ctl.Tag.ToString())].Bounds.Height)
							ctl.Visible = false;
						else
							ctl.Visible = true;
						ctl.Top = listView1.Items[int.Parse(ctl.Tag.ToString())].Bounds.Top;
						ctl.Left = listView1.Items[int.Parse(ctl.Tag.ToString())].Bounds.Left + listView1.Columns[0].Width + + listView1.Columns[1].Width + 1;
						ctl.Width = listView1.Columns[2].Width - 2;
					}
                    if (obj.GetType().ToString() == "ProjectPublisher2016.UserControl1")
					{
						UserControl1 ctl = (UserControl1)obj;
						if(listView1.Items[int.Parse(ctl.Tag.ToString())].Bounds.Top < listView1.Items[int.Parse(ctl.Tag.ToString())].Bounds.Height)
							ctl.Visible = false;
						else
							ctl.Visible = true;
						ctl.Top = listView1.Items[int.Parse(ctl.Tag.ToString())].Bounds.Top;
						if(ctl.isAccept)
							ctl.Left = listView1.Items[int.Parse(ctl.Tag.ToString())].Bounds.Left + listView1.Columns[0].Width / 2 - ctl.Width / 2;
						else
							ctl.Left = listView1.Items[int.Parse(ctl.Tag.ToString())].Bounds.Left + listView1.Columns[0].Width + listView1.Columns[1].Width / 2 - ctl.Width / 2;;
					}
					if(obj.GetType().ToString() == "System.Windows.Forms.Label")
					{
						Label lbl = (Label)obj;
						if(listView1.Items[int.Parse(lbl.Tag.ToString())].Bounds.Top < listView1.Items[int.Parse(lbl.Tag.ToString())].Bounds.Height)
							lbl.Visible = false;
						else
							lbl.Visible = true;
						lbl.Top = listView1.Items[int.Parse(lbl.Tag.ToString())].Bounds.Top;
					}
				}
			}
		}

		private void listView1_DoubleClick(object sender, System.EventArgs e)
		{
			FormTaskInformation frmTaskInformation = new FormTaskInformation(taskList[int.Parse(listView1.SelectedItems[0].Tag.ToString())], pj);
			frmTaskInformation.txtApprovalNotes.Text = listView1.SelectedItems[0].SubItems[2].Text;
			frmTaskInformation.ShowDialog();
			if(frmTaskInformation.DialogResult == DialogResult.OK)
				listView1.SelectedItems[0].SubItems[2].Text = frmTaskInformation.txtApprovalNotes.Text;
			taskList[int.Parse(listView1.SelectedItems[0].Tag.ToString())].moderationNotes = frmTaskInformation.txtApprovalNotes.Text;
			frmTaskInformation.Dispose();
		}

		private void listView1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			
			
		}
		private void button1_Click(object sender, System.EventArgs e)
		{
			hasSelected = false;
			foreach(Object obj in listView1.Controls)
			{
                if (obj.GetType().ToString() == "ProjectPublisher2016.UserControl1")
				{
					UserControl1 ctl = (UserControl1)obj;
					if(ctl.checkBox1.Checked)
					{
						hasSelected = true;
						break;
					}
				}
			}
			if(!hasSelected)
			{
				DialogResult ret = MessageBox.Show("Updates have not been Accepted ot Rejected.  To Accept or Reject a task, check the task and click the Accept or Reject button.\n\nPress Ok to continue, or Cancel to stay and accept or reject tasks.","Confirm",MessageBoxButtons.OKCancel);
				if(ret == DialogResult.OK)
				{
					processTasks();
					this.DialogResult = DialogResult.Cancel;					
				}
			}
			else
			{
				processTasks();
				this.DialogResult = DialogResult.OK;
			}
		}

		private void processTasks()
		{
			int counter = 0;
			foreach(ListViewItem li in listView1.Items)
			{
				if(li.Tag.ToString() != "")
				{
                    if (li.Tag.GetType().ToString() == "ProjectPublisher2016.FormApproveEntTasks+UUpdateTaskItem")
					{
						UUpdateTaskItem ti = (UUpdateTaskItem)li.Tag;
						if(ti.approvalstatus != 0)
							counter++;
						if(ti.approvalstatus == 2)
						{
							if(ti.task == null)
							{
								ti.task = pj.Tasks.Add(ti.taskname,System.Reflection.Missing.Value);
								ti.task.Start = ti.startDate;
								ti.task.Finish = ti.finishDate;
								ti.task.PercentComplete = ti.percentComplete;
								ti.taskuid = ti.task.Guid;
							}
							foreach(EPMLivePublisher.UpdateFieldItem fi in ti.updatefields)
							{
								if(fi.fieldId != 0)
								{
									if(fi.value != null)
									{
										ti.task.Notes = ti.notes;
										try
										{
                                            if (fi.fieldId == 188743715)
                                                ti.task.Start = fi.value;
                                            else if (fi.fieldId == 188743716)
                                                ti.task.Finish = fi.value;
                                            else if (fi.fieldId == 188743709)
                                                ti.task.Duration = fi.value;
                                            else
											    ti.task.SetField((Microsoft.Office.Interop.MSProject.PjField)fi.fieldId,fi.value);
										}
										catch(Exception ex)
										{
											//MessageBox.Show("Unable to set field '" + ((Microsoft.Office.Interop.MSProject.PjField)fi.fieldId).ToString().Substring(6) + "' to value '" + fi.value + "' on task '" + ti.task.Name + "': " + ex.Message);
										}
									}
								}
							}
						}
					}
				}
			}

			approvedTasks = new EPMLivePublisher.TaskApprovalItem[counter];
			counter = 0;
			foreach(ListViewItem li in listView1.Items)
			{
				if(li.Tag.ToString() != "")
				{
                    if (li.Tag.GetType().ToString() == "ProjectPublisher2016.FormApproveEntTasks+UUpdateTaskItem")
					{
						UUpdateTaskItem ti = (UUpdateTaskItem)li.Tag;
						if(ti.approvalstatus != 0)
						{
							approvedTasks[counter] = new EPMLivePublisher.TaskApprovalItem();
							approvedTasks[counter].approvalNotes = ti.approvalnotes;
							approvedTasks[counter].approvalStatus = ti.approvalstatus;
							approvedTasks[counter].listItemId = ti.listItemId;
							if(ti.task != null)
								approvedTasks[counter].taskuid = ti.task.Guid;
							counter++;
						}
					}
				}
			}
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
		
		}
		private void checkBox1_CheckedChanged(object sender, System.EventArgs e)
		{
			checkBox2.Checked = false;
			foreach(Object obj in listView1.Controls)
			{
                if (obj.GetType().ToString() == "ProjectPublisher2016.UserControl1")
				{
					UserControl1 ctl = (UserControl1)obj;

					if(!ctl.isAccept)
						ctl.checkBox1.Checked = false;
					else
					{
						if(checkBox1.Checked)
						{
							((FormApproveEntTasks.UUpdateTaskItem)ctl.lItem.Tag).approvalstatus = 2;
							ctl.checkBox1.Checked = true;
						}
						else
							ctl.checkBox1.Checked = false;
					}

					((FormApproveEntTasks.UUpdateTaskItem)ctl.lItem.Tag).approvalstatus = 2;
				}
			}
		}

		private void button5_Click(object sender, System.EventArgs e)
		{
			try
			{
				string subject = Connection.getProjectName(pj) + ": " + Connect.taskList[int.Parse(listView1.SelectedItems[0].Tag.ToString())].taskname;
				string body = "\n\nSite Address: " + Connection.url;
				MapiMailMessage message = new MapiMailMessage(subject, body);

				string[] ress = Connect.taskList[int.Parse(listView1.SelectedItems[0].Tag.ToString())].resource.Split(";#".ToCharArray());
				if(ress.Length >2)
					for(int i=2;i<ress.Length;i=i+4)
						foreach(Microsoft.Office.Interop.MSProject.Resource res in pj.Resources)
							if(res.Name == ress[i])
								message.Recipients.Add(res.EMailAddress);

				message.ShowDialog();

			}
			catch(System.Exception){}

		}

		private void checkBox2_CheckedChanged(object sender, System.EventArgs e)
		{
			checkBox1.Checked = false;
			foreach(Object obj in listView1.Controls)
			{
                if (obj.GetType().ToString() == "ProjectPublisher2016.UserControl1")
				{
					UserControl1 ctl = (UserControl1)obj;
					if(ctl.isAccept)
						ctl.checkBox1.Checked = false;
					else
					{
						if(checkBox2.Checked)
						{
							ctl.checkBox1.Checked = true;
							((FormApproveEntTasks.UUpdateTaskItem)ctl.lItem.Tag).approvalstatus = 1;
						}
						else
							ctl.checkBox1.Checked = false;
					}
				}
			}
		}

		private void listView1_GiveFeedback(object sender, System.Windows.Forms.GiveFeedbackEventArgs e)
		{
			MessageBox.Show(e.Effect.ToString());
		}
	}
}
