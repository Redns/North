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