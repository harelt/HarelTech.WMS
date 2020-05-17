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
    app.loader = document.createElement('ion-loading');
    app.loader.message = 'Please wait...';
    app.loader.spinner = 'lines';
    //loading.duration = 2000;

    document.body.appendChild(app.loader);
    await app.loader.present();

    const { role, data } = await app.loader.onDidDismiss();
    console.log('Loading dismissed!');
};
app.hideLoader = async function () {
    app.loader.dismiss();
};


window.app.login = async function (username, password) {
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

    try {

        login(config).then(
            (loginFunctions) => {
                console.log(loginFunctions);
                loginFunctions.companies().then(function myfunction(data) {
                    cons.log(data.Company);
                });
                console.log('Your are in!! Enjoy!');
                axios({
                    method: 'get',
                    url: app.url + '/Account/Login/?handler=Auth',
                    //headers: { 'Content-Type': 'application/json' },
                    data: { userName: username, password: password }
                }).then(function (response) {

                    customElements.define('product-p', class Product extends HTMLElement {
                        connectedCallback() {

                            $("#app-nav").html(response.data);
                        }
                    });

                    app.nav.push('product-p').then(done => {
                        app.nav.componentOnReady();
                    });


                }).catch(function (error) {
                    console.log(error);
                }).then(function () {
                    app.hideLoader();
                });
            },
            reason => {
                console.log(reason.message);
            }
        );

    }
    catch (err) {
        console.log(err);
    }
    //const promise = Promise.resolve(login(config));
    //promise.then(loginSuccess, loginFail);

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
    let toast = document.createElement('ion-toast');
    toast.color = "danger";
    toast.message = message;
    toast.duration = 4000;

    document.body.appendChild(toast);
    return toast.present();
};