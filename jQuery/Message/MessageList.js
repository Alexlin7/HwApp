$(function () {
    $(".editMessage").click(function () {

        // 解除重複綁定事件
        $(".sentMessage").off("click");

        // 把留言內容改為輸入方塊
        $(this).closest('tr').prev('tr').prev('tr').children('td:eq(1)').html("<input class='editInput'>");

        // 把修改留言改為送出修改
        $(this).html("送出修改");

        // 解除綁定事件
        $(".editMessage").off("click");

        // 送出修改的Class與功能
        $(this).removeClass("editMessage");
        $(this).addClass("sentMessage");

        // 撰寫sentMessage的功能
        $(".sentMessage").click(function () {

            var $editInput = $(this).closest('tr').prev('tr').prev('tr').children('td:eq(1)').children('input').val();
            console.log($editInput);

            // 撰寫修改功能的傳輸參數
            var parameter = $(".sentMessage").next('button').attr('onclick');
            // 將parameter中取得的參數 '@Url.Action("DeleteMessage", "Blog", new { M_Id = item.M_Id, A_Id = item.A_Id })';return false;"
            // 修改為 '@Url.Action("UpdateMessage", "Blog", new { M_Id = item.M_Id, A_Id = item.A_Id })';return false;"
            // 因此即可將此按鈕路徑轉為指向 BlogController 中的 UpdateMessage Action
            parameter = parameter.replace("DeleteMessage", "UpdateMessage");
            // 把第三個參數放進去

            parameter = parameter.replace("';return false;", "&Content=" + $editInput + "';return false;");
            // 添加一個假Button 
            // display: none 隱藏Button
            $(this).parent("td").append("<button class='ImFaker' style='display:none'>我是假的</button>");
            // 添加onclick屬性到button上
            $(this).next('button').next('button').attr("onclick", parameter);
            // 設定完成之後使用語法再點一次
            $(".ImFaker").click();
        });
    });
});
