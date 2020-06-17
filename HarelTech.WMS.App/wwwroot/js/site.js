'use strict';
window.app = {};
app.url = "https://localhost:44387";
app.company = "";
app.warhouseId = 0;
app.priorityUrl = "https://harel-tech-dev.trio-cloud.com";
app.tasksForm;
app.tasksSubForm;
app.taskId = 0;

app.profile = {
    "id": 0,
    "user_login": null,
    "user_name": null,
    "user_id": 0,
    "user_group": 0
};

window.addEventListener("load", () => {
    function handleNetworkChange(event) {
        if (navigator.onLine) {
            //document.body.classList.remove("offline");
            app.network("You are online..", "info");
        } else {
            app.network("No network connection. You are offline.", "error");
        }
    }
    window.addEventListener("online", handleNetworkChange);
    window.addEventListener("offline", handleNetworkChange);
});


function priorityReady () {
    console.log("Priority Ready");
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

function showMessage(message) {
    if (message.type === "information" || message.type === "warning") {
        //all good
        $("#modalConfirm #p_message").html(message.message);
        $("#modalConfirm").modal("show");
        $("#modalConfirm #confirm_btn").on("click", function () {
            if (message.type === "warning") {
                //then confirm
                message.form.warningConfirm(1);
            }
            message.form.endCurrentForm();
            //trigger click to return previous screen
            $("#btn_back")[0].click()
        });
    }
    else {
        //rollback, delete lots by HWMS_ITASK
        $("#modalConfirm #p_message").html(message.message);
        $("#modalConfirm").modal("show");
        $("#modalConfirm #confirm_btn").on("click", function () {
            tasksForm.warningConfirm(0);
            $.ajax({
                url: app.url + "Tasks/Transaction/?handler=DeleteTaskLots&taskId=" + app.taskId,
                type: 'DELETE',
                success: function (result) {
                    tasksForm.endCurrentForm();
                    app.hideLoader();
                    $("#btn_back")[0].click()
                }
            });

        });
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

window.app.presentToast = async function presentToast(message, top, msgtype) {
    if (top) {
        toastr.options = {
            "closeButton": false,
            "debug": false,
            "newestOnTop": false,
            "progressBar": true,
            "positionClass": "md-toast-bottom-center",
            "preventDuplicates": false,
            "onclick": null,
            "showDuration": 300,
            "hideDuration": 1000,
            "timeOut": 2000,
            "extendedTimeOut": 1000,
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        }
    }
    else
    toastr.options = {
        "positionClass": "md-toast-bottom-center",
        }
    toastr[msgtype](message);

};

window.app.network = function (message, msgtype) {
    toastr.options = {
        "positionClass": "md-toast-bottom-center",
        "autohide": false,
        "closeButton": true
    }
    toastr[msgtype](message);
};

window.app.taskForm = async function (username, password, filter, company, qty, taskId) {
    app.taskId = taskId;
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
            formStart('HWMS_ITASKS', showMessage, null, { "company": company }, 1).then(function (tasksForm) {
                console.log("Form done...");
                tasksForm.setSearchFilter(filter).then(function () {
                    console.log("Search filter done...");
                    tasksForm.getRows(1).then(function () {
                        console.log("Row fetched done done...");
                        //tasksForm.fieldUpdate("HWMS_AUSERLOGIN", username, null, function (msg) { alert(msg); });
                        tasksForm.fieldUpdate("HWMS_COMPLETEDQTY", qty, null, function (msg) { alert(msg); });
                        //tasksForm.fieldUpdate("HWMS_ITASKSTATUS", "F", null, function (msg) { alert(msg); });
                        tasksForm.fieldUpdate("HWMS_ITASKSTATUS", "F");
                        tasksForm.saveRow(0, null, showMessage);
                        app.hideLoader();
                    });
                });
            });
        },
        reason => {
            app.presentToast(reason.message, null, error);
            //app.presentToast(reason.message);
            console.log(reason.message);
        }
    );

};

window.app.PostLotTransaction = async function (rows, taskType) {
    //app.tasksForm.isAlive(function () { alert('live'); }, function () { alert('dead'); }) 
    app.tasksForm.startSubForm("HWMS_ITASKLOTS", null, null,
        function (subform) {
            app.tasksSubForm = subform;
            window.app.AddLotRows(rows, taskType);
        },
        function (msg) { alert("sub form HWMS_ITASKLOTS" + msg); });

};

window.app.PostSerialTransaction = async function () {

    app.tasksForm.startSubForm("HWMS_ITASKSER", null, null, null, function (msg) { alert("sub form HWMS_ITASKSER " + msg); }).then(function (subForm) {
        app.tasksSubForm = subForm;
    });
};

window.app.AddLotRows = async function (rows, taskType) {
    app.tasksForm.startSubForm("HWMS_ITASKLOTS", null, null,
        function (subform) {
            //app.tasksSubForm = subform;
            rows.forEach(item => {
                //app.tasksSubForm.newRow().then(function (row) {
                    if (taskType === 3) {
                        subform.fieldUpdate("HWMS_EXPDATE", item.ExpDate);
                    }
                subform.fieldUpdate("HWMS_LOTNUMBER", item.HWMS_ELOTNUMBER);
                subform.fieldUpdate("HWMS_LOTQUANTITY", item.Quantity);
                subform.fieldUpdate("HWMS_FROMBIN", item.FROMBIN);
                subform.fieldUpdate("HWMS_TOBIN", item.TOBIN);
                //});
                subform.saveRow(0).then(function (reason) { console.log(reason); });
            });

            subform.endCurrentForm();
            //app.tasksForm.fieldUpdate("HWMS_ITASKSTATUS", 'F').then(function () {
            //    app.tasksForm.endCurrentForm();
            //    app.tasksSubForm = null;
            //    app.tasksForm = null;
            //});
            
        },
        function (msg) { alert("sub form HWMS_ITASKLOTS" + msg); });
    
    //window.app.CloseSubForm(app.subform.na);
}

window.app.AddSerialRows = async function (rows, taskType) {

    for (var i = 0; i < rows.length; i++) {
        app.tasksSubForm.newRow(null, function (msg) { alert("sub form AddSerialRow" + msg); })
        if (taskType !== 3)
            app.tasksSubForm.fieldUpdate("HWMS_SERNUM", rows[i].HWMS_ELOTNUMBER);
        else
            app.tasksSubForm.fieldUpdate("HWMS_NSERNUM", rows[i].HWMS_ELOTNUMBER);

        app.tasksSubForm.fieldUpdate("HWMS_FROMBIN", rows[i].FROMBIN);
        app.tasksSubForm.fieldUpdate("HWMS_TOBIN", rows[i].TOBIN);
        app.tasksSubForm.fieldUpdate("HWMS_TOBIN", rows[i].Quantity);
        app.tasksSubForm.saveRow(0, null, function (msg) { alert("sub form save AddSerialRow: " + msg); })
    }
    
};

window.app.CloseSubForm = async function (formName) {
    app.tasksSubForm.endCurrentForm(null, function (msg) {
        alert("close form " + formName + ": " + msg);
    });
};

window.app.CloseForm = async function (finilize) {
    if (finilize) {
        app.tasksForm.fieldUpdate("HWMS_ITASKSTATUS", "F", null, function (msg) { alert("HWMS_ITASKSTATUS: " + msg); });
        app.tasksForm.saveRow(0, null, function (msg) { alert("HWMS_ITASKSTATUS: " + msg); });
    }
    app.tasksSubForm.endCurrentForm(null, function (msg) {
        alert("close sub form " + formName + ": " + msg);
    });
};

