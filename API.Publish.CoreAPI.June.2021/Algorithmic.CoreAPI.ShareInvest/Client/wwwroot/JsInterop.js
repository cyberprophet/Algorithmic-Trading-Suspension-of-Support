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
	},
	recall: function (param)
	{
		return document.getElementById(param).value;
	}
};
function number(obj)
{
	var rgx1 = /\D/g;
	var num01;
	var num02;
	num01 = obj.value;
	num02 = num01.replace(rgx1, "");
	num01 = comma(num02);
	obj.value = num01;
};
function comma(inNum)
{
	var rgx2 = /(\d+)(\d{3})/;
	var outNum;
	outNum = inNum;

	while (rgx2.test(outNum))
	{
		outNum = outNum.replace(rgx2, '$1' + ',' + '$2');
	}
	return outNum;
};