﻿function ResourceToolsBuildTeam() {

    var url = SP.PageContextInfo.get_webServerRelativeUrl()
    if (url == "/")
        url = "";

    var options = SP.UI.$create_DialogOptions();
    options.url = url + "/_layouts/epmlive/buildteam.aspx";
    options.showMaximized = true;
    SP.UI.ModalDialog.showModalDialog(options);
}

function ResourceToolsBuildTeamEnabled()
{
    return true;
}