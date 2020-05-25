'use strict';
window.app = {};
app.url = "https://localhost:44387";
app.company = "";
app.warhouseId = 0;
app.priorityUrl = "https://harel-tech-dev.trio-cloud.com";
app.tasksForm;
app.tasksSubForm;

app.profile = {
    "id": 0,
    "user_login": null,
    "user_name": null,
    "user_id": 0,
    "user_group": 0
};

function priorityReady () {
    console.log("Priority Ready");
    $("#priority_ready").val(true).trigger('change');
}

//function priorityReady() {
//    console.log("Priority Ready");
//    //  login({
//    //      username: 'tabula', password: 'LioHarel1!',
//    //      url: 'https://prioritysoftware.github.io/', tabulaini: 'tabula.ini',
//    //      language: 3, company: 'HarelTech', appname: 'EMS'})
//    //      .then(function () {
//    //          console.log("Login Success");
//    //    //return formStart('DOCUMENTS_P', showMessage, updateFields);
//    //}).then(function(form) {
//    //              myForm = form;
//    //        });
//}

//XMLHttpRequest.prototype.origOpen = XMLHttpRequest.prototype.open;
//XMLHttpRequest.prototype.open = function () {
//    this.origOpen.apply(this, arguments);
//    this.setRequestHeader('Access-Control-Allow-Origin', '*');
//};
app.showLoader = async function () {
    $("#mdb-preloader").show();
};
app.hideLoader = async function () {
    $("#mdb-preloader").hide();
};


window.app.signin = async function (username, password) {
    app.showLoader();
    var config = {
        url: app.priorityUrl,
        tabulaini: "tabula.ini",
        language: 3,
        //company: "smoukr",
        //profile: {
        //    company: 'smoukr'
        //},
        appname: 'wmsapp',
        username: username,
        password: password
    };

    var cmps = [];
    login(config).then(
        (loginFunctions) => {
            console.log(loginFunctions);
            loginFunctions.companies().then(function myfunction(data) {
                for (var i = 0; i < data.Company.length; i++) {
                    var cmp = { dname: data.Company[i].dname, title: data.Company[i].title };
                    cmps.push(cmp);
                }
                $("#frmLogin #Companies").val(JSON.stringify(cmps));
                //console.log(JSON.stringify(cmps));
                //console.log('Your are in!! Enjoy!');
                $("#frmLogin").submit();
            });
            
        },
        reason => {
            $("#login_error_message").html(reason.message);
            $("#login_error").show();
            //app.presentToast(reason.message);
            console.log(reason.message);
        }
    );
    
    
};

window.app.showMessage = function (message) {
    if (message.type !== "warning") {
        alert(message.message);
    } else {
        if (confirm(message.message)) {
            message.form.warningConfirm(1);
        } else {
            message.form.warningConfirm(0);
        }
    }
};

window.app.updateFields = function updateFields(result, frmName) {
    if (result[frmName]) {
        var fields = result[frmName][1];
        for (var fieldName in fields) {
            var el = document.getElementById(fieldName);
            if (el) el.value = fields[fieldName];
        }
    }
};

window.app.presentToast = async function presentToast(message) {
    toastr.options = {
        "positionClass": "toast-bottom-left"
    }
    toastr["error"](message);

};

window.app.taskForm = async function (username, password, filter, company) {
    debugger;
    var config = {
        url: app.priorityUrl,
        tabulaini: "tabula.ini",
        language: 3,
        //company: "smoukr",
        //profile: {
        //    company: 'smoukr'
        //},
        appname: 'wmsapp',
        username: username,
        password: password
    };
    login(config).then(
        (loginFunctions) => {
            console.log("Login done...");
            formStart('HWMS_ITASKS', showMessage, null, { "company": company }, 1).then(function (form) {
                console.log("Form done...");
                app.tasksForm = form;
                app.tasksForm.setSearchFilter(filter).then(function () {
                    console.log("Search filter done...");
                    app.tasksForm.getRows(1).then(function () {
                        console.log("Row fetched done done...");
                        app.tasksForm.fieldUpdate("HWMS_ITASKSTATUS", "A", null, function (msg) { alert(msg); });
                        app.tasksForm.fieldUpdate("HWMS_AUSERLOGIN", username, null, function (msg) { alert(msg); });
                        app.tasksForm.saveRow(0, null, function (msg) { alert(msg); });
                        app.hideLoader();
                    });
                });
            })
        },
        reason => {
            $("#login_error_message").html(reason.message);
            $("#login_error").show();
            //app.presentToast(reason.message);
            console.log(reason.message);
        }
    );
    
}

windows.app.PostLotTransaction = async function () {

    app.tasksForm.startSubForm("HWMS_ITASKLOTS", null, null, null, function (msg) { alert("sub form HWMS_ITASKLOTS" + msg); }).then(function (subForm) {
        app.tasksSubForm = subForm;
    });
}

windows.app.PostSerialTransaction = async function () {

    app.tasksForm.startSubForm("HWMS_ITASKSER", null, null, null, function (msg) { alert("sub form HWMS_ITASKSER " + msg); }).then(function (subForm) {
        app.tasksSubForm = subForm;
    });
}

windows.app.AddLotRow = async function (row, taskType) {

    app.tasksSubForm.newRow(null, function (msg) { alert("sub form " + msg); });
    if (taskType !== 3)
        app.tasksSubForm.fieldUpdate("HWMS_ELOTNUMBER", "");
    else {
        app.tasksSubForm.fieldUpdate("HWMS_NLOTNUMBER", "");
        app.tasksSubForm.fieldUpdate("HWMS_EXPDATE", "dd/MM/yyyy");
    }
        
    app.tasksSubForm.fieldUpdate("HWMS_LOTQUANTITY", "");
    app.tasksSubForm.fieldUpdate("HWMS_FROMBIN", "");
    app.tasksSubForm.fieldUpdate("HWMS_TOBIN", "");
    app.tasksSubForm.saveRow(0, null, function (msg) { alert("sub form save AddLotRow: " + msg); })
}

windows.app.AddSerialRow = async function (row, taskType) {
    app.tasksSubForm.newRow(null, function (msg) { alert("sub form AddSerialRow" + msg); })

    if (taskType !== 3)
        app.tasksSubForm.fieldUpdate("HWMS_SERNUM", "");
    else 
        app.tasksSubForm.fieldUpdate("HWMS_NSERNUM", "");
    
    app.tasksSubForm.fieldUpdate("HWMS_FROMBIN", "");
    app.tasksSubForm.fieldUpdate("HWMS_FROMBIN", "");
    app.tasksSubForm.fieldUpdate("HWMS_TOBIN", "");
    app.tasksSubForm.saveRow(0, null, function (msg) { alert("sub form save AddSerialRow: " + msg); })
}

windows.app.CloseSubForm = async function (formName) {
    app.tasksSubForm.endCurrentForm(null, function (msg) {
        alert("close sub form " + formName + ": " + msg); });
}

windows.app.CloseForm = async function (finilize) {
    if (finilize) {
        app.tasksForm.fieldUpdate("HWMS_ITASKSTATUS", "F", null, function (msg) { alert("HWMS_ITASKSTATUS: " + msg); });
        app.tasksForm.saveRow(0, null, function (msg) { alert("HWMS_ITASKSTATUS: " + msg); });
    }
    app.tasksSubForm.endCurrentForm(null, function (msg) {
        alert("close sub form " + formName + ": " + msg);
    });
}
