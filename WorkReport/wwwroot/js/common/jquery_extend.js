var jquery_extend = {
    openwin: function () {

    }
}
//日期转换
Date.prototype.Format = function (fmt) {
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "H+": this.getHours(), //小时 
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}
//接收url中参数
function getQueryStringByName(name) {
    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    if (result == null || result.length < 1) {
        return "";
    }
}
/**
* 使用post方式
* @param url
* @param params
*  params is javascript plain object
*/
function download(url, params) {
    var tid = guid();
    var iframeID = "download_iframe" + tid;
    var $iframe, iframeDoc, iframeHtml;
    if (($iframe = $('#' + iframeID)).length === 0) {
        $iframe = $("<iframe id='" + iframeID +"'" +
            " style='display: none' src='about:blank'></iframe>"
        ).appendTo("body");
    }
    iframeDoc = $iframe[0].contentWindow || $iframe[0].contentDocument;
    if (iframeDoc.document) {
        iframeDoc = iframeDoc.document;
    }
    iframeHtml = "<html><head></head><body><form method='POST' action='" + url + "'>"
    Object.keys(params).forEach(function (key) {
        iframeHtml += "<input type='hidden' name='" + key + "' value='" + params[key] + "'>";
    });
    iframeHtml += "</form></body></html>";
    iframeDoc.open();
    iframeDoc.write(iframeHtml);
    $(iframeDoc).find('form').submit();
}
/**
 *获取id
 */
function guid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}
//获取文件后缀名
var getFileSuffix = function (fileName) {
    var suffixName = '';
    var fileNameSplit = fileName.split('.');
    if (fileNameSplit.length > 0) {
        suffixName = fileNameSplit[fileNameSplit.length - 1];
    }
    return suffixName;
}

//变量不为空，转换为字符返回。
function isNullReturnStr(val) {
    if (val == null || val == undefined) {
        return val
    } else {
        return val.toString();
    }
}
$.extend(String.prototype, {
    isPositiveInteger: function () {
        return (new RegExp(/^[1-9]\d*$/).test(this));
    },
    isInteger: function () {
        return (new RegExp(/^\d+$/).test(this));
    },
    isNumber: function (value, element) {
        return (new RegExp(/^-?(?:\d+|\d{1,3}(?:,\d{3})+)(?:\.\d+)?$/).test(this));
    },
    trim: function () {
        return this.replace(/(^\s*)|(\s*$)|\r|\n/g, "");
    },
    startsWith: function (pattern) {
        return this.indexOf(pattern) === 0;
    },
    endsWith: function (pattern) {
        var d = this.length - pattern.length;
        return d >= 0 && this.lastIndexOf(pattern) === d;
    },
    replaceSuffix: function (index) {
        return this.replace(/[0−9]+[0−9]+/, '[' + index + ']').replace('#index#', index);
    },
    trans: function () {
        return this.replace(/&lt;/g, '<').replace(/&gt;/g, '>').replace(/&quot;/g, '"');
    },
    replaceAll: function (os, ns) {
        return this.replace(new RegExp(os, "gm"), ns);
    },
    replaceTm: function ($data) {
        if (!$data) return this;
        return this.replace(RegExp("({[A-Za-z_]+[A-Za-z0-9_]*})", "g"), function ($1) {
            return $data[$1.replace(/[{}]+/g, "")];
        });
    },
    replaceTmById: function (_box) {
        var $parent = _box || $(document);
        return this.replace(RegExp("({[A-Za-z_]+[A-Za-z0-9_]*})", "g"), function ($1) {
            var $input = $parent.find("#" + $1.replace(/[{}]+/g, ""));
            return $input.val() ? $input.val() : $1;
        });
    },
    isFinishedTm: function () {
        return !(new RegExp("{[A-Za-z_]+[A-Za-z0-9_]*}").test(this));
    },
    skipChar: function (ch) {
        if (!this || this.length === 0) { return ''; }
        if (this.charAt(0) === ch) { return this.substring(1).skipChar(ch); }
        return this;
    },
    isValidPwd: function () {
        return (new RegExp(/^([_]|[a-zA-Z0-9]){6,32}$/).test(this));
    },
    isValidMail: function () {
        return (new RegExp(/^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$/).test(this.trim()));
    },
    isSpaces: function () {
        for (var i = 0; i < this.length; i += 1) {
            var ch = this.charAt(i);
            if (ch != ' ' && ch != "\n" && ch != "\t" && ch != "\r") { return false; }
        }
        return true;
    },
    isPhone: function () {
        return (new RegExp(/(^([0-9]{3,4}[-])?\d{3,8}(-\d{1,6})?$)|(^[0−9]3,4[0−9]3,4\d{3,8}(\d1,6\d1,6)?$)|(^\d{3,8}$)/).test(this));
    },
    isUrl: function () {
        return (new RegExp(/^[a-zA-z]+:\/\/([a-zA-Z0-9\-\.]+)([-\w .\/?%&=:]*)$/).test(this));
    },
    isExternalUrl: function () {
        return this.isUrl() && this.indexOf("://" + document.domain) == -1;
    },
});