//Constants and DOM variables
const határidőElements = document.getElementsByClassName("határidő")
const határidő = "2022-01-16"
//const határidő = "2021-12-5"   for testing changes after the time limit has elapsed
let győztesInput = null
let calculateId = null
let licitBtn = null
let overElements = null

if(document.getElementsByClassName("possibly-over")) {
    overElements = document.getElementsByClassName("possibly-over")
}

if(document.getElementById("győztes")) {
    győztesInput = document.getElementById("győztes").value;
}
if(document.getElementById("licit-btn")) {
    licitBtn = document.getElementById("licit-btn")
}

//Date variables
const dateNow = Date.now()
const dateDeadline = new Date(határidő + "Z")
let diffInSec = (dateDeadline - dateNow) / 1000
if(diffInSec < 0) diffInSec = 0;

function changeText(newText) {
    for(let i = 0; i < határidőElements.length; i++) {
        határidőElements[i].innerHTML = newText
    }
}

function győztes() {
    if(licitBtn) {
       licitBtn.disabled = true
       licitBtn.classList.add("over")

       for(let i = 0; i < overElements.length; i++) {
           overElements[i].classList.add("over")
       }
    }

    if(győztesInput) {
        const div = document.createElement("div")
        const span = document.createElement("span")
        const textNode = document.createTextNode("Győztes: " + győztesInput)
        span.appendChild(textNode)
        div.appendChild(span)
        div.classList.add("input-container2")
        span.classList.add("licit-span")
        document.querySelector(".licit-input-container").appendChild(div)
    }
}

/*Calculate days,hour,minutes,seconds, change text in dom,then lower the difference by one 
  sec and if there is no more difference stop calling the function and close the bidding*/
function calculateTime() {
    let difference = diffInSec

    if(diffInSec <= 0) {
        if(calculateId) {
            clearInterval(calculateId)
        }

        changeText("árverés lezárva")
        győztes()
    }
    else {
        const days = Math.floor(difference / 86400)
        difference -= days * 86400;
    
        const hours = Math.floor(difference / 3600) % 24
        difference -= hours * 3600;
    
        const minutes = Math.floor(difference / 60) % 60
        difference -= minutes * 60;
    
        const seconds = Math.floor(difference % 60)
    
        let text = `${days} nap ${hours}:${minutes}:${seconds}`
        changeText(text)
    
        diffInSec--
    }
}

//Call the function manually, so there is no delay at the beginning, after that call regularly
calculateTime()
if(diffInSec > 0) {
    calculateId = setInterval(calculateTime, 1000)
}

