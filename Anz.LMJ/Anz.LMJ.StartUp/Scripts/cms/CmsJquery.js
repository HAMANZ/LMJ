
function readURL(input) {
    console.log(input.files[0].name);
    $("#BackGroundImage").val(input.files[0].name);
    $("#BackgroundImage").val(input.files[0].name);
    $("#ArticleImage").val(input.files[0].name);

    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#blah').attr('src', e.target.result);
            $("#ImageBase").val(e.target.result)
        }
        reader.readAsDataURL(input.files[0]);
    }

}

function readURLBanner(input) {
    console.log(input.files[0].name);
    $("#BackGroundImageBanner").val(input.files[0].name);
    $("#BackgroundImageBanner").val(input.files[0].name);
    $("#ArticleImage").val(input.files[0].name);
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#blahBackGroundImageBanner').attr('src', e.target.result);
            $("#ImageBase").val(e.target.result)
        }
        reader.readAsDataURL(input.files[0]);
    }

}
  


function readURL1(input) {
    console.log(input.files[0].name);
    //$("#BackGroundImage").val(input.files[0].name);
    //$("#BackgroundImage").val(input.files[0].name);
    $("#ArticleImage").val(input.files[0].name);

    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#blahBanner').attr('src', e.target.result);
            //$("#BannerImages").val(e.target.result)
        }
        reader.readAsDataURL(input.files[0]);
    }

}

$(document).ready(function () {

    $("#imgInp").change(function () {
        readURL(this);
    });

    $("#imgInpBanner").change(function () {
        readURLBanner(this);
    });
    //var editor = "";

    $(".editArticle").on("click", function (event) {
        event.preventDefault();
        var Id = $("#Id").val();
        var Key = $(this).attr("data-row-type");
        var Value = $(this).attr("data-row-value");
        //console.log(Id);
        //console.log(Key);
        //console.log(Value.replace(/</g, '*').replace(/>/g, '#'));

        $.ajax({
            url: '/Admin/Edit2',
            type: "POST",
            data: { Id: Id, Key: Key, Value: Value.replace(/</g, '*').replace(/>/g, '#') },
        }).done(function (result) {
            console.log(result);
            $("#EditPageModal .modal-body").html(result);
            $('#EditPageModal').modal('show');
            $('#summernote').summernote();
            //editor = CKEDITOR.replace('ckeditor', { height: '380px', startupFocus: true });
        });
    });

    $("#SaveBtn").on("click", function () {
        //console.log("clicked");
        //console.log($("#Key").val());
        //console.log(editor.getData());
        var key = $("#Key").val();
        console.log(key);
        var html = $('#summernote').summernote('code');
        console.log(html);
        //$("." + key).html(editor.getData());
        //$("." + key).val(editor.getData());

        $("." + key).html(html);
        $("." + key).val(html);
        $('#EditPageModal').modal('hide');
    });

    $("#BannerPublish").on("click", function () {
        console.log("Banner Publish Clicked");
        //Title
        var Title = $("input[name=Title]").val();
        if (Title != null) {
            $("input[name=Title]").val(Title.replace(/</g, '*').replace(/>/g, '#'));
        }
        //SubTitle
        var SubTitle = $("input[name=SubTitle]").val();
        if (SubTitle != null) {
            $("input[name=SubTitle]").val(SubTitle.replace(/</g, '*').replace(/>/g, '#'));
        }
        //Sentence
        var Sentence = $("input[name=Sentence]").val();
        if (Sentence != null) {
            $("input[name=Sentence]").val(Sentence.replace(/</g, '*').replace(/>/g, '#'));
        }
        //Description
        var Description = $("input[name=Description]").val();
        if (Description != null) {
            $("input[name=Description]").val(Description.replace(/</g, '*').replace(/>/g, '#'));
        }
        //FullDescription
        var FullDescription = $("input[name=FullDescription]").val();
        if (FullDescription != null) {
            $("input[name=FullDescription]").val(FullDescription.replace(/</g, '*').replace(/>/g, '#'));
        }


        //ArticleTitle
        var ArticleTitle = $("input[name=ArticleTitle]").val();
        if (ArticleTitle != null) {
            $("input[name=ArticleTitle]").val(ArticleTitle.replace(/</g, '*').replace(/>/g, '#'));
        }
        //ArticleDescription
        var ArticleDescription = $("input[name=ArticleDescription]").val();
        if (ArticleDescription != null) {
            $("input[name=ArticleDescription]").val(ArticleDescription.replace(/</g, '*').replace(/>/g, '#'));
        }
        //ArticleFullDescription
        var ArticleFullDescription = $("input[name=ArticleFullDescription]").val();
        if (ArticleFullDescription != null) {
            $("input[name=ArticleFullDescription]").val(ArticleFullDescription.replace(/</g, '*').replace(/>/g, '#'));
        }


        //BannerTitle
        var BannerTitle = $("input[name=BannerTitle]").val();
        if (BannerTitle != null) {
            $("input[name=BannerTitle]").val(BannerTitle.replace(/</g, '*').replace(/>/g, '#'));
        }
        //BannerDescription
        var BannerDescription = $("input[name=BannerDescription]").val();
        if (BannerDescription != null) {
            $("input[name=BannerDescription]").val(BannerDescription.replace(/</g, '*').replace(/>/g, '#'));
        }

        //console.log("Title" + $("input[name=Title]").val());
        //console.log("SubTitle"+$("input[name=SubTitle]").val());
        //console.log("Sentence" + $("input[name=Sentence]").val());
        //console.log("Description" + $("input[name=Description]").val());
        //BannerForm

        $(".BannerForm").submit();


    });
});