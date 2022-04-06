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