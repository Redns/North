/**
 * 复制文本到粘贴板
 * @param {待复制的文本} content
 */
function copyTextToClipboard(content) {
    var aux = document.createElement("input");
    aux.setAttribute("value", content);
    document.body.appendChild(aux);
    aux.select();
    document.execCommand("copy");
    document.body.removeChild(aux);
    return content;
}