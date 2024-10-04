/// <reference path="../lib/jquery/dist/jquery.js" />
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
                _errcallback(err);
            }
        });
    }
    static HttpPost(_url, _data, _callback, _errcallback) {
        return $.ajax({
            url: _url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(_data),
            success: function (data) {
                _callback(data);
            },
            error: function (err) {
                _errcallback(err);
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

    static SearchItemGroupInitialize() {
        let input = document.createElement("input");
        $("#image-search");
    }
}
