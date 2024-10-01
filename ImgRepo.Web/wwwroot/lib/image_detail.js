/// <reference path="jquery/dist/jquery.js" />

var ImgRepoHelperInstance = null;

class ImgRepoHelper {
    static CreateTagItem(name) {
        let newItem = document.createElement("span");
        let item = $(newItem);
        item.addClass("img-tag");
        item.html(`
        <input type="hidden" value="${name}" />
        <span>${name}</span>
        <button class="btn" type="button" onclick="ImgRepoHelperInstance.Remove(this)"></button>`);
        return newItem;
    }
    static CreateCatItem(name) {
        let newItem = document.createElement("span");
        let item = $(newItem);
        item.addClass("img-cat");
        item.html(`
        <input type="hidden" value="${name}" />
        <span>${name}</span>
        <button class="btn" type="button" onclick="ImgRepoHelperInstance.Remove(this)"></button>`);
        return newItem;
    }
    constructor(baseUrl, imageId) {
        this.BaseUrl = baseUrl;
        this.ImageId = imageId;
    }
    LoadImage() {
        $.ajax({
            url: this.BaseUrl + "api/query/image/file/" + this.ImageId,
            type: "GET",
            success: function (data) {
                $("#img-view").attr("src", `data:image/${data.format};base64,` + data.base64);
            }
        });
    }
    LoadTags() {
        $.ajax({
            url: this.BaseUrl + `api/query/tags/` + this.ImageId,
            type: "GET",
            success: function (data) {
                let tagContainer = $("#img-tag-container");
                data.forEach(item => {
                    tagContainer.append(ImgRepoHelper.CreateTagItem(item.name));
                });
            }
        });
    }
    LoadCats() {
        $.ajax({
            url: this.BaseUrl + `api/query/categories/` + this.ImageId,
            type: "GET",
            success: function (data) {
                let catContainer = $("#img-cat-container");
                data.forEach(item => {
                    catContainer.append(ImgRepoHelper.CreateCatItem(item.name));
                });
            }
        });
    }
    Add(_btn) {
        let btn = $(_btn);
        let parent = btn.parent("div");
        let name = parent.find(".img-repo-input").val();
        let container = parent.find(".img-repo-item-container");
        let _url = this.BaseUrl;
        let callback = null;
        if (container[0].id == 'img-tag-container') {
            _url += `api/save/tag/add/${this.ImageId}/${name}`;
            callback = ImgRepoHelper.CreateTagItem;
        } else if (container[0].id == 'img-cat-container') {
            _url += `api/save/cat/add/${this.ImageId}/${name}`;
            callback = ImgRepoHelper.CreateCatItem;
        }
        $.ajax({
            url: _url,
            type: "GET",
            success: function (data) {
                console.log(data);
                container.append(callback(data.name));
            }
        });
    }
    Remove(_btn) {
        let btn = $(_btn);
        let parent = btn.parent();
        let name = parent.find("input").val();
        let _url = this.BaseUrl;
        if (parent[0].classList.contains("img-tag")) {
            _url += `api/save/tag/remove/${this.ImageId}/${name}`;
        } else if (parent[0].classList.contains("img-cat")) {
            _url += `api/save/cat/remove/${this.ImageId}/${name}`;
        }
        $.ajax({
            url: _url,
            type: "GET",
            success: function (data) {
                console.log(data);
            }
        }).then(() => {
            parent.remove();
        });
    }
}
