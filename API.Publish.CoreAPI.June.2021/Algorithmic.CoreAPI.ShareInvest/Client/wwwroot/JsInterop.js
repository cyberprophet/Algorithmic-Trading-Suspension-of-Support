window.JsFunctions =
{
	move: function (param)
	{
		window.scrollBy({
			behavior: "smooth",
			left: 0,
			top: param
		});
	},
	selector: function (param)
	{
		var element = document.getElementById(param);
		element.scrollIntoView({
			behavior: "smooth",
			block: "center",
			inline: "start"
		});
	},
	download: function (param)
	{
		var link = document.createElement('a');
		link.href = param;
		document.body.appendChild(link);
		link.click();
		document.body.removeChild(link);
	}
};