var step = 0;
var velocity = 3;
$(document).ready(function() {
    var elmnt = document.getElementById('bedSituationContainer');

    if (elmnt) {
        elmnt.scrollLeft = step;
        boxWidth = (225 + 5) * ($('#bedSituationContainer').children().length - 2);

        setInterval(function(){
            elmnt.scrollLeft += step;
            var element = document.getElementsByClassName('btLeft')[0];

            if (elmnt.scrollLeft == 0) {
                element.style.display = "none";
            } else {
                element.style.display = "block";
            }
            var limite = (boxWidth-$(document).width()) - elmnt.scrollLeft;
        }, 1);
    }
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

$(document).ready(function() {
    
    $('.bt_close_filter').click(function() {
		$('#box_filter').toggle('slide');
		$('#bt_filter').show();
	});
	
	$('#bt_filter').click(function(){
		$('#box_filter').show('slide', {direction:'left'}, 450);
		$('#bt_filter').hide();
	});

});
var step3 = 0;
$(document).ready(function() {
    
    //var elmnt = document.getElementById('pannelActivitiesContainer');
    //elmnt.scrollLeft = step3;
    $('.scrolling').scrollTop();
    $('.arrow_right').click(function() {
        $('.jquery-modal').removeClass('hideModal');
    });
    $('.arrow_right_encaminhamento').click(function() {
        $('.encaminhamento').removeClass('hideModal');
    });
    $('.close-modal').click(function() {
        $('.jquery-modal').addClass('hideModal');
    });
    
    setInterval(function(){
        //elmnt.scrollLeft += step3;
        //var element = document.getElementsByClassName('btLeft')[1];
        //if (elmnt.scrollLeft == 0) {
        //    element.style.display = "none";
        //} else {
        //    element.style.display = "block";
        //}
        $('.scrolling').height($(window).height() - 250);
        $('.boxIntern').height($(window).height() - 250);
        $('#box_filter').height($(window).height() - 108);
        $('.situation .boxIntern').height($(window).height() - 268);
    }, 1);
});

function showCoordsIntern(event) {
    var x = event.clientX;
    if (x < 300) {
        step3 = -velocity;
    } 
    if (x > (document.getElementById("pannelActivities").offsetWidth - 300)) {
        step3 = velocity;
    } 
}

function stopshowCoordsIntern() {
    step3 = 0;
}


$(document).ready(function() {
	if (document.getElementById('loginForm')) {
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
		document.addEventListener("click", closeAllSelect);
	}
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
});

var step2 = 0;
$(document).ready(function() {

    var pathname = window.location.pathname;
    if (pathname.indexOf('altas.html') > 0) {
        $('body').addClass('removeHorizontalScroll');
    }
    
    var elmnt = document.getElementById('pannelActivitiesContainer');
    if (elmnt) {
        elmnt.scrollLeft = step2;

        setInterval(function(){
            elmnt.scrollLeft += step2;
            var element = document.getElementsByClassName('btLeft')[1];
            if (elmnt.scrollLeft == 0) {
                element.style.display = "none";
            } else {
                element.style.display = "block";
            }
        }, 1);
    }
});

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