$(document).ready(function () {
    $("#btnSubmit").click(function () {
        var val = $("#txtInputStr").val();
        $.ajax({
            url: "/binary/CheckValidBinaryString",
            data: JSON.stringify(val),
            contentType: "application/json",
            dataType: "json",
            type: "POST",
            async: false,
            success: function (result) {
                alert(result);
            },
            error: function () {
                alert("Error occured while proceesing your request. Pls try again.");
            }
        });
    });
});