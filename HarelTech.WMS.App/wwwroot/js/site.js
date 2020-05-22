'use strict';
window.app = {};
app.url = "https://localhost:44387";
app.company = "";
app.warhouseId = 0;

app.profile = {
    "id": 0,
    "user_login": null,
    "user_name": null,
    "user_id": 0,
    "user_group": 0
};

function priorityReady() {
    console.log("Priority Ready");
    //  login({
    //      username: 'tabula', password: 'LioHarel1!',
    //      url: 'https://prioritysoftware.github.io/', tabulaini: 'tabula.ini',
    //      language: 3, company: 'HarelTech', appname: 'EMS'})
    //      .then(function () {
    //          console.log("Login Success");
    //    //return formStart('DOCUMENTS_P', showMessage, updateFields);
    //}).then(function(form) {
    //              myForm = form;
    //        });
}

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
        url: "https://harel-tech-dev.trio-cloud.com",
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