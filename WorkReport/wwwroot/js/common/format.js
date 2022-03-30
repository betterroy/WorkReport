﻿function init() {
    var contentIndex;
    layui.use(['form', 'laydate', 'layedit'], function () {
        var laydate = layui.laydate;
        var form = layui.form;
        var layedit = layui.layedit;

        laydate.render({    //日期控件初始化
            elem: '#ReportTime',
            type: 'date'
        });
        $('#ReportTime').val(getRecentDay(0));

        form.render('select', 'UserId');

        form.on('submit(ok)', function (data) {     //点击保存按钮
            data.field.Content = layedit.getContent(contentIndex);
            $.ajax({
                url: "/UReport/SaveReport",
                dataType: 'json',
                async: false,
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(data.field),
                method: "post",
                success: function (response) {
                    if (response.Code==200) {
                        var index = parent.layer.getFrameIndex(window.name);//获取窗口索引
                        parent.layer.close(index);//关闭弹出层
                    } else {
                        layer.msg("数据保存失败");
                    }
                },
                error: function () {
                    layer.msg("数据保存失败");
                }
            });
            return false;
        });


        //构建一个默认的编辑器
        contentIndex = layedit.build('Content', {
            tool: ['strong' //加粗
                , 'italic' //斜体
                , 'underline' //下划线
                , 'del' //删除线
                , '|' //分割线
                , 'left' //左对齐
                , 'center' //居中对齐
                , 'right' //右对齐
                , 'link' //超链接
                //, 'face' //表情
            ]
            , height: 266
        });


        if (_data && _data.hasOwnProperty('ID')) {
            form.val('example', {
                "ID": _result.ID
                , "UserId": _result.UserId
                , "ReportTime": _result.ReportTime
                , "Content": _result.Content
            });
            layedit.setContent(contentIndex, _result.Content, false);   //true，是追加模式，false，赋值模式
        }
        else {
            var defaultContent = "<p><b>今天：</b></p><p><br></p><p><b>明天：</b></p><p><br></p>";
            layedit.setContent(contentIndex, defaultContent, false);   //true，是追加模式，false，赋值模式
        }
    });
}

var _result;
var _data;
function SetData(data) {
    _data = data;
    if (data && _data.hasOwnProperty('ID')) {
        $.ajax({
            url: "/UReport/GetUReportByID/" + data.ID,
            data: { ID: data.ID },
            method: "GET",
            async: false,
            success: function (result) {
                _result = result;
                init();
            }
        });
    }
}