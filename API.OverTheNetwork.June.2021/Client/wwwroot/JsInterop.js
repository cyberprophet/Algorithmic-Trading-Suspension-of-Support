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
	}
};