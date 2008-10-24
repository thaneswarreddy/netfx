/*
  @author : Brian J. Cardiff
  @usage :
	$.status.showInfo(msg);
	$.status.showWarning(msg);
	$.status.showError(msg);
 */
(function($) {
	function statusAreaElement() {
		return $('#status');
	}
	
	function addRemoveButton() {
			var removeButton = document.createElement("a");
			$(removeButton).text("x").click($.status.clear).attr("href", "#");
			statusAreaElement().append("  ").append(removeButton);
			statusAreaElement().corner('5px');
	}
	
	$.extend({ 
		status : {
			showInfo : function(str) { 
				statusAreaElement().text(str).attr('class', 'status_info'); 
				addRemoveButton();
			},
			showWarning : function(str) { 
				statusAreaElement().text(str).attr('class', 'status_warning'); 
				addRemoveButton();
			},
			showError : function(str) { 
				statusAreaElement().text(str).attr('class', 'status_error'); 
				addRemoveButton();
			},
			clear : function(){
				statusAreaElement().text('').attr('class', 'status_invisible');
				return false; // cancels link navigation.
			}
		}
	});
	
	$(document).ready(function(){				
		$.status.clear();
	});
})(jQuery);
