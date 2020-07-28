$('#show').click(function()
{
    $('#show').css('display','none');
    $('#data').show();
    $('#hide').show();
});

$('#hide').click(function()
{
    $('#hide').css('display','none');
    $('#data').hide();
    $('#show').show();
});

function Keyboard() {
    $('#keyboard').jkeyboard({
        layout: "english",
        input: $('#input_field_1')
    });


    $(".input_box").focus(function () {
        $('#keyboard').jkeyboard("setInput", this);
    });
}