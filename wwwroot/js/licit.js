//DOM
const ajánlat_inputElement = document.getElementById("ajánlat")
const aktuális_árElement = document.getElementById("aktuális-ár")
const kikiáltási_árElement = document.getElementById("kikiáltási-ár")
 
//Needed integer values from DOM elements
const aktuális_ár = parseInt(aktuális_árElement.innerHTML)
const kikiáltási_ár = parseInt(kikiáltási_árElement.innerHTML)

if(kikiáltási_ár > aktuális_ár) {
    ajánlat_inputElement.value = kikiáltási_ár
}
else {
    ajánlat_inputElement.value = aktuális_ár + 1
}

ajánlat_inputElement.addEventListener("change", (e) => {
    let currentAjánlat = parseInt(e.target.value)
    let ár = 0

    if(kikiáltási_ár > aktuális_ár) {
        ár = kikiáltási_ár
    }
    else {
        ár = aktuális_ár +1
    }

    if(currentAjánlat < ár) ajánlat_input.value = ár
})

