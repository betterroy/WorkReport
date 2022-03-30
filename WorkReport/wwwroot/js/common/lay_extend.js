var lay_extend = {

    openwin: function (parameter) {    //弹出一个窗口，传入集合
        var width = (parameter.width > 0 ? parameter.width : 893) + 'px';
        var height = (parameter.height > 0 ? parameter.height : 600) + 'px';
        layer.open({
            type: 2,
            title: parameter.title,
            shadeClose: true,
            shade: 0.3,
            maxmin: true, //开启最大化最小化按钮
            area: [width, height],
            content: '/Home/Redirect?path=' + parameter.page_path,
            success: function (layero, index) {

                //var body = layer.getChildFrame('body', index);
                //console.log(body.html()) //得到iframe页的body内容
                //body.find('#fileId').val('Hi，我是从父页来的')

                var iframeWin = window[layero.find('iframe')[0]['name']]; //得到iframe页的窗口对象，执行iframe页的方法：iframeWin.method();
                iframeWin.SetData(parameter.data);

            },
            end: function () {
                parameter.cancel();
            }
        });
    }

}


