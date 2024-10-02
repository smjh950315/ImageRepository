/// <reference path="jquery/dist/jquery.js" />

class ImgRepoCommon {
    static OpenEdit(_btn) {
        let group = $(_btn).closest(".img-repo-edit-group");
        group.find(".img-repo-input").css("display", "");
        group.find(".img-repo-save-btn").css("display", "");
        group.find(".img-repo-cancel-btn").css("display", "");
        group.find(".img-repo-edit-btn").css("display", "none");
    }
    static CloseEdit(_btn) {
        let group = $(_btn).closest(".img-repo-edit-group");
        group.find(".img-repo-input").css("display", "none");
        group.find(".img-repo-input").val("");
        group.find(".img-repo-save-btn").css("display", "none");
        group.find(".img-repo-cancel-btn").css("display", "none");
        group.find(".img-repo-edit-btn").css("display", "");
    }
    static SaveEdit(_btn, callback_to_save) {
        let group = $(_btn).closest(".img-repo-edit-group");
        let input_value = group.find(".img-repo-input").val();
        callback_to_save(input_value);
        ImgRepoCommon.CloseEdit(_btn);
    }
    static HttpGet(_url, _callback, _errcallback) {
        return $.ajax({
            url: _url,
            type: "GET",
            success: function (data) {
                _callback(data);
            },
            error: function (err) {
                _callback(err);
            }
        });
    }
    static HttpPost(_url, _data, _callback, _errcallback) {
        return $.ajax({
            url: _url,
            type: "POST",
            contentType: "application/json",
            data: _data,
            success: function (data) {
                _callback(data);
            },
            error: function (err) {
                _callback(err);
            }
        });
    }
    static GetImgRepoItem(_inner, _removecallback) {
        let newItem = document.createElement("span");
        let item = $(newItem);
        item.addClass("img-repo-item");
        item.html(_inner);
        let newLink = document.createElement("a");
        newLink.innerHTML = "-";
        $(newLink).click(() => {
            _removecallback(newItem);
            $(newItem).remove();
        })
        item.append(newLink);
        return newItem;
    }
}

class ImgRepoIndexHelper {
    static CreateThumbItem(x) {
        let newItem = document.createElement("div");
        let item = $(newItem);
        item.addClass("img-repo-thumb");
        item.addClass("col-3");
        item.html(`
        <a href="/Image/Index?id=${x.imageId}">
        <img src="data:image/${x.format};base64,${x.base64}" />
        <div class="text-center">${x.imageName}</div>
        </a>
        `);
        return newItem;
    }
    static LoadThumbnails(_baseUrl) {
        let container = $("#index-thumbnail-container");
        ImgRepoCommon.HttpPost(_baseUrl + `api/query/thumbnails/`, null, (data) => {
            data.forEach(x => {
                container.append(ImgRepoIndexHelper.CreateThumbItem(x));
            });
        });
    }
}

class ImgRepoImageIndexHelper {
    static EditImageName(_btn, _baseUrl) {
        let imgId = Number($("#image-id").text());
        ImgRepoCommon.SaveEdit(_btn, (value) => {
            let _url = _baseUrl + `api/save/rename/image/${imgId}/${value}`;
            ImgRepoCommon.HttpGet(_url, (res) => {
                $(_btn).closest("#image-index-info-group").find(".img-repo-name").html(value);
                ImgRepoCommon.CloseEdit(_btn);
            }, (err) => { alert(err); });
        });
    }
    static AddNewItem(_btn, _baseUrl, _itemName) {
        let imgId = Number($("#image-id").text());
        ImgRepoCommon.SaveEdit(_btn, (value) => {
            ImgRepoCommon.HttpGet(_baseUrl + `api/save/${_itemName}/add/${imgId}/${value}`, (data) => {
                let newItem = ImgRepoCommon.GetImgRepoItem(`<span>${data.name}</span>`, (item) => {
                    ImgRepoCommon.HttpGet(_baseUrl + `api/save/${_itemName}/remove/${imgId}/${data.name}`, (e) => {
                        $(item).remove();
                    });
                });
                $(_btn).closest(".img-repo-attr-group").find(".img-repo-item-container").append(newItem);
            }, (err) => { });
        });
    }
    static LoadItems(_baseUrl, _itemName, _itemNameSingle, _container) {
        let imgId = Number($("#image-id").text());
        ImgRepoCommon.HttpGet(_baseUrl + `api/query/${_itemName}/` + imgId, (datas) => {
            let container = $(_container);
            datas.forEach(data => {
                let newItem = ImgRepoCommon.GetImgRepoItem(`<span>${data.name}</span>`, (item) => {
                    ImgRepoCommon.HttpGet(_baseUrl + `api/save/${_itemNameSingle}/remove/${imgId}/${data.name}`, (e) => {
                        $(item).remove();
                    });
                });
                container.append(newItem);
            });
        });
    }
    static AddNewTag(_btn, _baseUrl) {
        ImgRepoImageIndexHelper.AddNewItem(_btn, _baseUrl, "tag");
    }
    static AddNewCategory(_btn, _baseUrl) {
        ImgRepoImageIndexHelper.AddNewItem(_btn, _baseUrl, "category");
    }
    static LoadImage(_baseUrl) {
        let imgId = Number($("#image-id").text());
        ImgRepoCommon.HttpGet(_baseUrl + "api/query/image/file/" + imgId, (data) => {
            $("#image-view").attr("src", `data:image/${data.format};base64,` + data.base64);
        });
    }
    static LoadTags(_baseUrl) {
        ImgRepoImageIndexHelper.LoadItems(
            _baseUrl,
            "tags",
            "tag",
            $("#image-index-tag-group").find(".img-repo-item-container")[0]);
    }
    static LoadCategories(_baseUrl) {
        ImgRepoImageIndexHelper.LoadItems(
            _baseUrl,
            "categories",
            "category",
            $("#image-index-category-group").find(".img-repo-item-container")[0]);
    }
}

