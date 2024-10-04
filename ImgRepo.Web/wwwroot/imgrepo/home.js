/// <reference path="common.js" />
class ImgRepoIndexHelper {
    static CreateThumbItem(x) {
        let newItem = document.createElement("a");
        let item = $(newItem);
        item.attr("href", "/Image/Index?id=" + x.imageId);
        item.addClass("img-repo-thumb");
        item.html(`
        <img src="data:image/${x.format};base64,${x.base64}" />
        <div class="img-repo-thumb-overlay">${x.imageName}</div>        
        `);
        return newItem;
    }
    static LoadThumbnails(_baseUrl, queryModel = null) {
        let container = $("#index-thumbnail-container");
        ImgRepoCommon.HttpPost(_baseUrl + `api/query/image/thumbnails/`, queryModel, (data) => {
            container.html("");
            data.forEach(x => {
                container.append(ImgRepoIndexHelper.CreateThumbItem(x));
            });
        }, (err) => {
            console.log(err);
            console.log(queryModel);
        });
    }
}

