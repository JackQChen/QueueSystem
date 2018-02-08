//获取Url参数
(function ($) {
    $.getUrlParam = function (name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
        var r = window.location.search.substr(1).match(reg);
        if (r != null)
            return unescape(r[2]);
        return null;
    }
})(jQuery);
//封装ajax请求
(function ($) {
    var mask = multiLine(function () {/*!
        <div class="handle-loading" count=0 style="
        background-image: url(data:image/gif;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAANSURBVBhXY/j3798WAAmnA6/CpVb7AAAAAElFTkSuQmCC);
        position: absolute;
        z-index: 999;
        ">
        <div style="
        height: 30px;
        overflow: auto;
        margin: auto;
        position: absolute;
        top: 0;
        left: 0;
        bottom: 0;
        right: 0;
        text-align: center;
        color: #0075FF;
        ">
        <div>正在加载数据...</div>
        </div>
        </div>
        */
    });
    var showMask = function (pCtl) {
        var loading = pCtl.find('.handle-loading');
        if (loading.length == 0) {
            if (pCtl == null)
                pCtl = $('body');
            pCtl.append(mask);
            loading = pCtl.find('.handle-loading');
            loading.offset(pCtl.offset());
            loading.width(pCtl.width());
            loading.height(pCtl.height());
        }
        loading.attr('count', Number(loading.attr('count')) + 1);
    };
    var removeMask = function (pCtl) {
        var loading = pCtl.find('.handle-loading');
        var count = Number(loading.attr('count')) - 1;
        loading.attr('count', count);
        if (count == 0)
            loading.remove();
    }
    $.handle = function (arg) {
        showMask(arg.mask);
        arg.type = "POST";
        if (arg.url == null)
            arg.url = "Handler.ashx";
        arg.dataType = "json";
        arg.data = JSON.stringify({
            invoke: arg.invoke,
            param: arg.param
        });
        var c = arg.complete;
        arg.complete = function () {
            removeMask(arg.mask);
            if (c != null)
                c();
        }
        ;
        var s = arg.success;
        arg.success = function (data) {
            if (data.action != null)
                eval(data.action);
            else {
                if (s != null)
                    s(data);
            }
        }
        ;
        if (arg.error == null)
            arg.error = function (XMLHttpRequest, textStatus, thrownError) {
                if (thrownError != '') {
                    alert(thrownError);
                }
            }
        $.ajax(arg);
    }
    ;
})(jQuery);
//多行字符串
function multiLine(fn) {
    var reg = /\/\*(.|\r|\n)+?\*\//g;
    return fn.toString().match(reg)[0].slice(5, -5);
}
//字符串格式化
String.prototype.format = function (args) {
    var result = this;
    if (arguments.length > 0) {
        if (arguments.length == 1 && typeof (args) == "object") {
            for (var key in args) {
                var reg = new RegExp("({" + key + "})", "g");
                result = result.replace(reg, args[key] == undefined ? '' : args[key]);
            }
        } else {
            for (var i = 0; i < arguments.length; i++) {
                if (arguments[i] != undefined) {
                    var reg = new RegExp("({)" + i + "(})", "g");
                    result = result.replace(reg, arguments[i] == undefined ? '' : arguments[i]);
                }
            }
        }
    }
    return result;
}
//时间转换
function DateConvert(dateString) {
    var tmp = /\d+(?=|\+)/.exec(dateString);
    return new Date(+tmp);
}
//时间格式化
Date.prototype.dateFormat = function (fmt) {
    var o = {
        "M+": this.getMonth() + 1,
        //月份
        "d+": this.getDate(),
        //日
        "H+": this.getHours(),
        //小时
        "m+": this.getMinutes(),
        //分
        "s+": this.getSeconds(),
        //秒
        "q+": Math.floor((this.getMonth() + 3) / 3),
        //季度
        "f+": this.getMilliseconds()//毫秒
    };
    if (/(y+)/.test(fmt))
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}
//表格赋值
$.fn.extend({
    SetForm: function (data) {
        for (var name in data) {
            if (data[name] == null)
                continue;
            var ctl = this.find("[name=" + name + "]");
            if (ctl.length == 0)
                continue;
            if (ctl.attr("type") == "radio" || ctl.attr("type") == "checkbox") {
                var val = data[name].split(',');
                for (var i = 0; i < ctl.length; i++) {
                    ctl[i].checked = $.inArray(ctl[i].value, val) > -1;
                }
            } else
                ctl.val(data[name]);
        }
    }
});
