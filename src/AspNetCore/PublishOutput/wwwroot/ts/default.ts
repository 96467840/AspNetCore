import UTS = require('../lib/unobtrusive-typescript/dist/unobtrusive-typescript');
//import UTS = require('unobtrusive-typescript');


let _app = new UTS.App();
_app
    //.AddCollection(new UTS.ItemCollection(<UTS.BaseParams>{ Selector: '.jsc-uts' }, _app))
    .AddDefaultCollections()
    .Run({
        before: function () { },
        ready: function (app: UTS.App) {
            UTS.Log('ready');
            app.FindCollectionBySelector('.jsc-uts').Invoke('hello');

            app.FindCollectionBySelector('.jsc-uts').Invoke('Say', null, ['Hello world', false]);

            /*let d = new UTS.Dialog(<UTS.DialogParams>{
                Show: true, Title: 'Test', Body: $('<p>Test dialog</p>'),
                Buttons: [new UTS.Button('Ok', function () { UTS.Log('Ok click'); }, 'btn btn-default js-close', 'button', { type: 'superbutton', 'data-s':'hello' })]
            }, 0, 'Close');/**/

        }
    });
