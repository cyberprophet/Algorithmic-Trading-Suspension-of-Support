window.JsFunctions =
{
	move: function (param)
	{
		window.scrollBy({
			behavior: "smooth",
			left: 0,
			top: param
		});
	}
};