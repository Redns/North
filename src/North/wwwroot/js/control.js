/**
 * 设置焦点
 * @param {控件id} id
 */
async function setFocus(id) {
    document.getElementById(id).focus();
    return id;
}


/**
 * 设置页面 Body 样式
 * @param {背景颜色}  backgroundColor
 * @param {过滤器}    filter
 */
async function setBodyStyle(backgroundColor, filter) {
    let bodyStyle = document.body.style;
    bodyStyle.backgroundColor = backgroundColor;
    bodyStyle.filter = filter;
    return "";
}