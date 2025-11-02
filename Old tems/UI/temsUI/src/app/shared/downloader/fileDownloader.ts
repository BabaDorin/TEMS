export class Downloader{
    public downloadFile(data, fileName: string) {
        const downloadedFile = new Blob([data], { type: data.type.toString() });
        var url = window.URL.createObjectURL(downloadedFile);
        var anchor = document.createElement("a");
        anchor.download = fileName;
        anchor.href = url;
        anchor.click();
    }
}