import $ = require('jquery');
import { App } from "./app";
import { Url } from "./url";
import { Ajax } from "./ajax";
import { Log } from './log';
import { Prompt, Dialog, DialogParams, Alert } from './dialog';

/**
 * Базовый тип входных параметров для всех компонентов
 */
export interface BaseParams {
    // привязка компонента на элементы $(Selector)
    Selector: string;
    // привязка на конкретные элементы
    Element: Element[];
}

/**
 * Базовый класс для всех компонентов
 */
export abstract class BaseItem<T extends BaseParams>
{
    // #region Свойства

    /**
     * Html элемент на который вешается компонент
     */
    Element: Element;
    element: JQuery;

    /**
     * Копия входных параметров
     */
    protected Params: T;

    /**
     * Ссылка на фабрику
     */
    protected Collection: BaseItemCollection<BaseItem<T>>;

    // #endregion

    constructor(element: Element, params: T, collection: BaseItemCollection<BaseItem<T>>) {
        let that = this;
        that.Params = params;
        that.Element = element;
        that.element = $(element);
        that.Collection = collection;
    }

    /**
     * обязательно удалять объекты (если поштучно) именно через эту функцию
     */
    public Remove(): void {
        this.Collection.RemoveElement(this.Element);
    }

    /**
     * Удаление компонента (Html элемент не удалем, если нужно удалить Html элемент,
     * то сделать это снаружи самостоятельно, строго после выполнения Destroy())
     * Обязательно реализовать в своих компонентах и удалить и отвязать все события. Все что создали в компоненте
     */
    public abstract Destroy(): void;
}

/**
 * Базовая коллекция для всех компонентов
*/
export abstract class BaseItemCollection<T extends BaseItem<BaseParams>>
{
    // #region Свойства

    // 
    /**
     * Уникальное имя коллекции. За уникальностью следим снаружи!
     * Возможно пригодится для поиска нужной коллекции на уровне приложения. Пока нигде не используется.
     * Пока для поиска используем Params.Selector
     */
    //Name: string;

    /**
     * Копия входных параметров
     */
    Params: BaseParams;

    /**
     * Коллекция компонентов
     */
    protected Collection: Array<T>;

    /**
     * Ссылка на приложение
     */
     public App: App;

    // #endregion

    constructor(params: BaseParams, app: App) {
        this.App = app;
        this.Params = params;
        this.Collection = new Array<T>();
    };

    // #region Методы

    /**
     * Выполняем привязку объекта к конкретному элементу ДОМ
     */
    private _bindCollection(elem: Element) {
        let that = this;
        // чтобы избежать повторного создания компонента 
        //Log('_bindCollection:', $(elem).data('__instance'));
        if ($(elem).data('__instance') == undefined) {
            let item = that.Сreate(elem, that.Params, that);
            $(elem).data('__instance', item);
            that.Collection.push(item);
        }
    }

    /**
     * Выполняем привязку компонентов в ДОМ
     */
    public BindCollection(): void {
        //Log('BindCollection:', this);
        let that = this;
        if (that.Params.Element) {
            that.Params.Element.forEach(function (elem: Element) {
                that._bindCollection(elem);    
            });
        }
        if (that.Params.Selector) {
            $(that.Params.Selector).each(function (index: Number, elem: Element) {
                that._bindCollection(elem);
            });
        }
    }

    /**
     * Выполняем вызов метода action на элементах elements (или на всех если elements=null) с параметрами params
     */
    public Invoke(action: string, elements: Array<Element> = null, params: Array<any> | any = null): Array<any>
    {
        let that = this;
        // так как мы юзаем apply при вызове функции то параметры надо загнать в массив
        if (!(params instanceof Array)) {
            params = [params];
        }
        let res: Array<any> = [];
        for (let i: number = 0, l: number = that.Collection.length; i < l; i++) {
            let _e = that.Collection[i];
            let e: any = _e;
            if (elements != null && elements.indexOf(_e.Element) < 0) continue;
            if (typeof e[action] === 'function') {
                // чтобы вызвать, например, метод с 2 аргументами 
                // collection.Invoke("method", [param1, param2]);
                // вызовет на каждом элементе
                // element.method(param1, param2);
                res.push(e[action].apply(e, params));
            } else {
                // если объект в коллекции несодержит функцию значит и все остальные по идее тоже ее не содержат (коллекция состоит из одинаковых объектов)
                // сделаем вывод сообщения об ошибке (мало ли опечатались)
                Log('Error: Action "' + action + '" in collection "' + (that.Params.Selector) + '" not founded.');
                break;
            }
        }
        return res;
    }

    /**
     * Удаляем единичный экземпляр компонента 
     */
    public RemoveElement(element: Element): void {
        for (let i: number = 0, l: number = this.Collection.length; i < l; i++) {
            if (this.Collection[i].Element == element) {
                $(this.Collection[i].Element).data('__instance', undefined);
                this.Collection[i].Destroy();
                this.Collection.splice(i, 1);
                break;
            }
        }
    }

    /**
     * Херим всю коллекцию целиком
     */
    public Destroy(): void {
        for (let i: number = 0, l: number = this.Collection.length; i < l; i++) {
            $(this.Collection[i].Element).data('__instance', undefined);
            this.Collection[i].Destroy();
        }
        this.Collection = null;
    }

    /**
     * В этом методе необходимо создать экземпляр компонента.
     * В этом методе просто вызывается конструктор экземпляра компонента
     */
    public abstract Сreate(element: Element, params: BaseParams, collection: BaseItemCollection<T>): T;

    // #endregion
}

enum EnumInsertinMode {
    Replace = 1,
    Before,
    After,
}

// ------------------------------------------------------------------------
/**
 * Основной компонент который навешивается на .jsc-uts селектор (аналог unobtrusive ajax от microsoft)
 */
export class Item extends BaseItem<BaseParams> {
    private data: any;

    //#region Свойства

    /**
     * Url который будет вызван по клику (перехват onclick события на элементе)
     * Читается с атрибута "data-OnClickURL"
     */
    OnClickURL: string;
    /**
     * Url который будет вызван для отправки формы (перехват onsubmit события на элементе)
     * Читается с атрибута "data-OnSubmitURL"
     */
    OnSubmitURL: string;

    /**
     * Метод отправки запроса
     * Читается с атрибута "data-Method"
     */
    Method: string;

    /**
     * По плану здесь должен быть текст или html который показывается при выполнении запроса
     * Читается с атрибута "data-Loading"
     */
    //Loading: string;

    /**
     * Запретить показ элемента лоадинг
     * Читается с атрибута "data-DisableLoading"
     */
    DisableLoading: boolean;

    /**
     * Сделать запрос у юзера перед отправкой запроса. Текст запроса.
     * Читается с атрибута "data-Prompt"
     */
    Prompt: string;
    /**
     * Сделать запрос у юзера перед отправкой запроса. Заголовок запроса. (По умолчанию без заголовка)
     * Читается с атрибута "data-PromptTitle"
     */
    PromptTitle: string;
    /**
     * Сделать запрос у юзера перед отправкой запроса. Текст на кнопке согласия. (по умолчанию "Да")
     * Читается с атрибута "data-PromptYes"
     */
    PromptOk: string;
    /**
     * Сделать запрос у юзера перед отправкой запроса. Текст на кнопке отмены. (по умолчанию "Нет")
     * Читается с атрибута "data-PromptNo"
     */
    PromptCancel: string;

    /**
     * Селектор контейнера для сообщений об ошибке. По умолчанию ошибка показывается в модальном окне.
     * Читается с атрибута "data-ErrorContainer"
     */
    ErrorContainer: string;
    /**
     * Селектор элементов которые надо скрыть перед запросом
     * Читается с атрибута "data-HideBefore"
     */
    HideBefore: string;
    /**
     * Селектор элементов которые надо скрыть после запроса
     * Читается с атрибута "data-HideAfter"
     */
    HideAfter: string;
    /**
     * Селектор элементов которые надо показать перед запросом
     * Читается с атрибута "data-ShowBefore"
     */
    ShowBefore: string;
    /**
     * Селектор элементов которые надо показать после запроса
     * Читается с атрибута "data-ShowAfter"
     */
    ShowAfter: string;
    /**
     * Селектор элементов которые надо очистить перед запросом
     * Читается с атрибута "data-ClearBefore"
     */
    ClearBefore: string;
    /**
     * Селектор элементов которые надо очистить после запроса
     * Читается с атрибута "data-ClearAfter"
     */
    ClearAfter: string;

    /**
     * Что нужно сделать с результатом выполнния запроса.
     * Читается с атрибута "data-InsertionMode"
     */
    InsertionMode: EnumInsertinMode; // before, after, replace (replace)
    /**
     * Селектор цели для выполнения операции InsertionMode
     * Читается с атрибута "data-target"
     */
    Target: string;

    //#endregion

    constructor(element: Element, params: BaseParams, collection: BaseItemCollection<Item>) {
        super(element, params, collection);

        this.Init().Render().BindCallback();
    }

    // #region Методы

    /**
     * Отвязка событий. Этот компонент ничего не созадет нового.
     */
    Destroy(): void {
        let element: Element = this.Element;

        // обязательно вернуть все в исходное состояние
        if (this.OnClickURL) {
            // обязательно чистим события, так как предусматриваем повтороный (возможно и не будем) вызов фабрики после изменений
            $(element).off('click');
        }

    }

    /**
     * чтение данных с атрибутов
     */
    Init(): Item {
        let element: Element = this.Element;

        this.OnClickURL = $(element).data('onclickurl');
        this.OnSubmitURL = $(element).data('onsubmiturl');
        this.Method = $(element).data('method');
        this.HideBefore = $(element).data('hidebefore');
        this.HideAfter = $(element).data('hideafter');
        this.ShowBefore = $(element).data('showbefore');
        this.ShowAfter = $(element).data('showafter');
        this.ClearBefore = $(element).data('clearbefore');
        this.ClearAfter = $(element).data('clearafter');
        let InsertionMode: string = $(element).data('insertionmode');
        this.Target = $(element).data('target');

        this.Prompt = $(element).data('prompt');
        this.PromptTitle = $(element).data('prompttitle');
        this.PromptOk = $(element).data('promptok');
        this.PromptCancel = $(element).data('promptcancel');

        this.DisableLoading = $(element).data('disableloading') == 1;
        //this.Loading = $(element).data('loading');
        //if (!this.Loading) this.Loading = '<div class="mdl-spinner mdl-js-spinner is-active loading"></div>';

        // get по умолчанию
        if (this.Method != 'post') this.Method = 'get';
        switch (InsertionMode) {
            case 'before': this.InsertionMode = EnumInsertinMode.Before; break;
            case 'after': this.InsertionMode = EnumInsertinMode.After; break;
            default: this.InsertionMode = EnumInsertinMode.Replace;
        }
        
        if (!this.PromptCancel) this.PromptCancel = 'Cancel';
        if (!this.PromptOk) this.PromptOk = 'Ok';
        return this;
    };

    /**
     * навешиваем стандартные обработчики
     */
    BindCallback(): Item {
        var element = this.Element;
        if (this.OnClickURL) {
            // обязательно чистим события, так как предусматриваем повтороный (возможно и не будем) вызов фабрики после изменений
            $(element).off('click');
            $(element).on('click', this.OnClick.bind(this));
        }
        /*if (this.OnSubmitURL) {
            $(element).on('submit', '.jsc-base', this.OnSubmit.bind(this));
        }/**/
        return this;
    };

    /**
     * отрисовка компонента
     */
    Render(): Item { return this; };

    Say(message: string, inDialog: boolean): void {
        if (inDialog) {
            new Alert('Say:', message, 'Close');
        } else {
            Log('Say:', message);
        }
    }

    /**
     * Показываем элемент "загрузка...""
     */
    ShowLoading(): void {
        if (this.DisableLoading) return;
        this.Collection.App.Loading([this.element]);
    };

    /**
     * Скрываем элемент "загрузка...""
     */
    HideLoading(): void {
        if (this.DisableLoading) return;
        this.Collection.App.Loaded([this.element]);
    };

    /**
     * отрисуем сообщение об ошибке
     */
    ShowError(message: string) {
        Log('ShowError(): ', message);
    };

    /**
     * Калбек на Submit
     */
    OnSubmit(): boolean {
        // сериализуем форму
        this.data = $(this.Element).serialize();
        this.onAction();
        return false;
    };

    /**
     * Калбек на клик
     */
    OnClick(): boolean {
        this.data = {};
        this.onAction();
        return false;
    };

    private promptDialog: Prompt = null;
    /**
     * Выполнение запроса
     * аргумент строка или объект
     */
    private onAction(): void {
        if (this.Prompt) {
            this.promptDialog = new Prompt(this.PromptTitle, this.Prompt, this.PromptOk, this.PromptCancel, this._onAction.bind(this));
            return;
        }
        this._onAction();
    };

    private _onAction(): void {
        var that = this;
        let data = that.data;
        if (that.promptDialog != null) that.promptDialog.Close();
        that.promptDialog = null;

        // что нужно сделать перед запросом. спросить юзера? показать колесико?
        if (that.HideBefore) {
            $(that.HideBefore).hide();
        }
        if (that.ClearBefore) {
            $(that.ClearBefore).html('');
        }
        if (that.ShowBefore) {
            $(that.ShowBefore).show();
        }
        that.ShowLoading();
        Ajax({
            url: this.OnClickURL,
            method: that.Method,
            dataType: 'json',
            data: data,
            success: function (data: any) {

                if (typeof data.error !== 'undefined') {
                    that.ShowError(data.error);
                } else {
                    if (that.Target) {
                        let html = '';
                        if (typeof data.html !== 'undefined') {
                            html = data.html;
                        }
                        // стандартное поведение на успех
                        switch (that.InsertionMode) {
                            case EnumInsertinMode.Before:
                                $(that.Target).before(html);
                                break;
                            case EnumInsertinMode.After:
                                $(that.Target).after(html);
                                break;
                            case EnumInsertinMode.Replace:
                                $(that.Target).replaceWith(html);
                                break;
                        }
                    }
                    if (that.HideAfter) {
                        $(that.HideAfter).hide();
                    }
                    if (that.ClearAfter) {
                        $(that.ClearAfter).html('');
                    }
                    if (that.ShowAfter) {
                        $(that.ShowAfter).show();
                    }
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                that.ShowError('При выполнении запроса произошла ошибка.  Проверьте соединение с интернетом.');
            },
            complete: function () { that.HideLoading(); }
        });
    };

    //#endregion
}

// экземпляр стандартной коллекции стандартного класса
export class ItemCollection extends BaseItemCollection<Item> {
    constructor(params: BaseParams, app: App) {
        super(params, app);
    };

    Сreate(element: Element, params: BaseParams, collection: ItemCollection): Item {
        return new Item(element, params, collection);
    }
}
