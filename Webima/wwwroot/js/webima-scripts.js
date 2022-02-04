
// Comprar -> Atualizar o valor total

$("#slct").on('change', function () {
    $("#total").text(function() {
        var preco = parseFloat($("#preco").text().replace(" €", "").replace(",", "."));
        var total = ((parseFloat($("#slct").val()) * preco).toFixed(2) + " €").replace(".", ",");
        return total;
    })
});