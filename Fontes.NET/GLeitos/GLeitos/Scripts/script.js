var x = 1;

var y = 2;

var step = 0;

var velocity = 3;

$(document).ready(function() {
	var customSelect, i, j, selElmnt, selectSelected, selectItems, c;

	customSelect = document.getElementsByClassName("custom-select");
	for (i = 0; i < customSelect.length; i++) {
		selElmnt = customSelect[i].getElementsByTagName("select")[0];
	  
		selectSelected = document.createElement("DIV");
		selectSelected.setAttribute("class", "select-selected");
		selectSelected.innerHTML = selElmnt.options[selElmnt.selectedIndex].innerHTML;
		customSelect[i].appendChild(selectSelected);
	  
		selectItems = document.createElement("DIV");
		selectItems.setAttribute("class", "select-items select-hide");
		  
	  	for (j = 1; j < selElmnt.length; j++) {
			c = document.createElement("DIV");
			c.innerHTML = selElmnt.options[j].innerHTML;
			c.addEventListener("click", function(e) {
				var y, i, k, s, h;
				s = this.parentNode.parentNode.getElementsByTagName("select")[0];
				h = this.parentNode.previousSibling;
				for (i = 0; i < s.length; i++) {
					if (s.options[i].innerHTML == this.innerHTML) {
						s.selectedIndex = i;
						h.innerHTML = this.innerHTML;
						y = this.parentNode.getElementsByClassName("same-as-selected");
						for (k = 0; k < y.length; k++) {
						y[k].removeAttribute("class");
						}
						this.setAttribute("class", "same-as-selected");
						break;
					}
				}
				h.click();
			});
			selectItems.appendChild(c);
	  	}
		customSelect[i].appendChild(selectItems);
	}
	
	document.getElementsByClassName("select-selected")[0].addEventListener("click", function(e) {
		e.stopPropagation();
		closeAllSelect(this);
		this.nextSibling.classList.toggle("select-hide");
		this.classList.toggle("select-arrow-active");
	});
	document.getElementsByClassName("select-selected")[1].addEventListener("click", function(e) {
		e.stopPropagation();
		closeAllSelect(this);
		this.nextSibling.classList.toggle("select-hide");
		this.classList.toggle("select-arrow-active");
	});

	function closeAllSelect(elmnt) {
	  var x, y, i, arrNo = [];
	  x = document.getElementsByClassName("select-items");
	  y = document.getElementsByClassName("select-selected");
	  for (i = 0; i < y.length; i++) {
		if (elmnt == y[i]) {
		  arrNo.push(i);
		} else {
		  y[i].classList.remove("select-arrow-active");
		}
	  }
	  for (i = 0; i < x.length; i++) {
		if (arrNo.indexOf(i)) {
		  x[i].classList.add("select-hide");
		}
	  }
	}

	document.addEventListener("click", closeAllSelect);
});


function showCoords(event) {
    var x = event.clientX;
    if (x < 300) {
        step = -velocity;
    }
    if (x > (document.getElementById("bedSituation").offsetWidth - 300)) {
        step = velocity;
    }
}

function stopshowCoords() {
    step = 0;
}

var step2 = 0;

function showCoordsPannel(event) {
    var x = event.clientX;
    if (x < 300) {
        step2 = -velocity;
    }
    if (x > (document.getElementById("pannelActivities").offsetWidth - 300)) {
        step2 = velocity;
    }
}

function stopShowCoordsPannel() {
    step2 = 0;
}