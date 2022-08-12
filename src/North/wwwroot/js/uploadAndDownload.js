/**
 * 调用浏览器下载器下载文件
 * @param {文件名称} filename
 * @param {文件链接} url
 */
async function download(filename, url) {
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = filename ?? '';
    anchorElement.click();
    anchorElement.remove();
}


/**
 * 上传文件并获取 Blob 对象
 * @param {待上传文件数据流} streamReference
 * @param {文件类型}        fileType
 */
async function upload(streamReference, fileType) {
    const arrayBuffer = await streamReference.arrayBuffer();
    return URL.createObjectURL(new Blob([arrayBuffer], { type: fileType }));
}


/**
 * 获取 Blob 文件数据流
 * @param {文件链接} url
 */
async function getBlobStream(url) {
    let blob = await fetch(url).then(r => r.blob());
    let buffer = await blob.arrayBuffer().then(buffer => buffer);
    return buffer;
}


async function destroy(url) {
    URL.revokeObjectURL(url);
}