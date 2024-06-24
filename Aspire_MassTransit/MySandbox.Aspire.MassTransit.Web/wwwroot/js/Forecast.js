"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/eventHub").build();

connection.on("SendFinalForecast", function (message) {
    var forecastParagraph = document.createElement("p");
    forecastParagraph.appendChild(document.createTextNode(JSON.stringify(message, null, 2)));

    document.getElementById("forecast-container").replaceChildren(forecastParagraph);
});

connection.start();