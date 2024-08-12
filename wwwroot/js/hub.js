//Variables
const connection = new signalR.HubConnectionBuilder().withUrl("/stateHub").build();
let ajánlat_input = null
let itemId = null
let licit_btn = null
let bids = null

//Save DOM at licitalas
if(document.getElementById("licit-btn")) {
    licit_btn = document.getElementById("licit-btn")
}

if(document.getElementById("itemIdForm")) {
    itemId = document.getElementById("itemIdForm").value
}

if(document.getElementById("ajánlat")) {
    ajánlat_input = document.getElementById("ajánlat")
}

if(document.getElementById("bids")) {
    bids = document.getElementById("bids")
}

//Add event listener at licitalas 
if(licit_btn != null) {
    licit_btn.addEventListener("click", () => {
        let ajánlat = ajánlat_input.value
        connection.invoke("SendUpdate", itemId, ajánlat )
    })
}

//Set up connetcion
connection.start().then(() => {
    console.log("Connected")
}).catch((err) => {
    return console.error(err.toString());
});

connection.on("ReceiveUpdate", (id, aktuális_ár) => {
    console.log("Receieved update: " + "id = " + id + " aktuális_ár = " + aktuális_ár)
    let aktuális_árElement = document.getElementById("aktuális-ár")
    
    //Change element id depending on the site and update element texts
    if(aktuális_árElement == null) {
        const element = "aktuális-ár " + id
        aktuális_árElement = document.getElementById(element)
    }
    aktuális_árElement.innerHTML = aktuális_ár

    if(bids != null) bids.innerHTML = parseInt(bids.innerHTML) + 1
    
});


