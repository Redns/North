/**
 * 获取设备信息
 * */
async function getDeviceInfo() {
    var os = "unknown"
    var description = "unknown"
    var deviceDescription = navigator.userAgent.split(/[(,)]/)[1]
    if (deviceDescription.includes('Android')) {
        var deviceDescriptionSlices = deviceDescription.split(';')
        for (var deviceDescriptionSlice of deviceDescriptionSlices) {
            if (deviceDescriptionSlice.includes('Android')) {
                os = deviceDescriptionSlice.trim()
            }
            else if (!deviceDescriptionSlice.includes('Linux')) {
                description = deviceDescriptionSlice.trim()
            }
        }
    }
    else if (deviceDescription.includes('Mac OS X')) {
        os = "OSX"
        description = deviceDescription.split(';')[0]
    }
    else if (deviceDescription.includes('Windows')) {
        os = "Windows"
    }
    else if (deviceDescription.includes('Linux')) {
        os = "Linux"
    }
    return {
        "os": os,
        "description": description,
        "screen": {
            "width": screen.width,
            "height": screen.height
        }
    }
}