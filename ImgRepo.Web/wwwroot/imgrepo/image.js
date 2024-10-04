/// <reference path="common.js" />
class ImgRepoImageIndexHelper {
    static EditImageName(_btn, _baseUrl) {
        let imgId = Number($("#image-id").text());
        ImgRepoCommon.SaveEdit(_btn, (value) => {
            let _url = _baseUrl + `api/save/image/name/`;
            ImgRepoCommon.HttpPost(_url, {id: imgId, value: value}, (res) => {
                $(_btn).closest("#image-index-info-group").find(".img-repo-name").html(value);
                ImgRepoCommon.CloseEdit(_btn);
            }, (err) => { alert(err); });
        });
    }
    static AddNewItem(_btn, _baseUrl, _objectName, _attrName) {
        let imgId = Number($("#image-id").text());
        ImgRepoCommon.SaveEdit(_btn, (value) => {
            let data = {id: imgId, value: value};
            ImgRepoCommon.HttpPost(_baseUrl + `api/save/${_objectName}/${_attrName}/add/`, data, (data) => {
                let newItem = ImgRepoCommon.GetImgRepoItem(`<span>${data.name}</span>`, (item) => {
                    ImgRepoCommon.HttpPost(_baseUrl + `api/save/${_objectName}/${_attrName}/remove/`, data, (e) => {
                        $(item).remove();
                    });
                });
                $(_btn).closest(".img-repo-attr-group").find(".img-repo-item-container").append(newItem);
            }, (err) => { });
        });
    }
    static LoadItems(_baseUrl, _objectName, _attrName, _attrNameSingle, _container) {
        let imgId = Number($("#image-id").text());
        ImgRepoCommon.HttpGet(_baseUrl + `api/query/${_objectName}/${_attrName}/` + imgId, (datas) => {
            let container = $(_container);
            console.log(datas);
            datas.forEach(data => {
                let newItem = ImgRepoCommon.GetImgRepoItem(`<span>${data.name}</span>`, (item) => {
                    ImgRepoCommon.HttpPost(_baseUrl + `api/save/${_objectName}/${_attrNameSingle}/remove/`,
                        { id: imgId, value: data.name },
                        (e) => {
                        $(item).remove();
                    });
                });
                container.append(newItem);
            });
        });
    }
    static AddNewTag(_btn, _baseUrl) {
        ImgRepoImageIndexHelper.AddNewItem(_btn, _baseUrl, "image", "tag");
    }
    static AddNewCategory(_btn, _baseUrl) {
        ImgRepoImageIndexHelper.AddNewItem(_btn, _baseUrl, "image", "category");
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
            "image",
            "tags",
            "tag",
            $("#image-index-tag-group").find(".img-repo-item-container")[0]);
    }
    static LoadCategories(_baseUrl) {
        ImgRepoImageIndexHelper.LoadItems(
            _baseUrl,
            "image",
            "categories",
            "category",
            $("#image-index-category-group").find(".img-repo-item-container")[0]);
    }
}
