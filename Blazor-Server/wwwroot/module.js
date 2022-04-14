/**
 * 拷贝信息到剪贴板
 * @param {待拷贝的信息} content
 */
async function CopyToClip(content) {
    var aux = document.createElement("input");
    aux.setAttribute("value", content);
    document.body.appendChild(aux);
    aux.select();
    document.execCommand("copy");
    document.body.removeChild(aux);
}


/**
 * 调用浏览器下载器下载文件
 * @param {文件名称} filename
 * @param {文件链接} url
 */
async function downloadFileFromStream(filename, url) {
    var a = document.createElement('a');
    var filename = filename;
    a.href = url;
    a.download = filename;
    a.click();
    window.URL.revokeObjectURL(url);
}


/**
 * 上传剪贴板图片
 * */
async function BindPasteEvent(imageUploadSizeLimit, imageUploadNumLimit) {
    document.addEventListener('paste', function (event) {
        var items = event.clipboardData && event.clipboardData.items;

        var imageUrls = "";
        var uploadImageNum = 0;
        var formdata = new FormData();

        // 遍历剪贴板，获取图片
        if (items && items.length) {
            for (var i = 0; i < items.length; i++) {
                if ((imageUploadNumLimit <= 0) || (uploadImageNum < imageUploadNumLimit)) {
                    if (items[i].type.indexOf('image') !== -1) {
                        var image = items[i].getAsFile();
                        if ((imageUploadSizeLimit <= 0) || (image.size <= imageUploadSizeLimit * 1024 * 1024)) {
                            formdata.append("images", image, image.name);
                            uploadImageNum++;
                        }
                    }
                }
            }
        }

        // 构造上传参数
        if (formdata.get("images") != null) {
            alert("开始上传!");

            var requestOptions = {
                method: 'POST',
                body: formdata,
                redirect: 'follow'
            };

            // 上传图片
            fetch(`api/image`, requestOptions)
                .then(response => {
                    response.json().then(data => {
                        for (var i in data.res) {
                            imageUrls += `http://${window.location.host}/${data.res[i]}\n`
                        }
                        CopyToClip(imageUrls.slice(0, -1));
                        alert("链接已复制至剪贴板!");
                    });
                })
                .then(result => {

                })
                .catch(error => console.log('error', error));
        }
        else {
            alert("剪贴板中未发现图片!");
        }
    });
}