using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ShareInvest.Pages
{
	public partial class IntroBase : ComponentBase
	{
		/*
		protected override async Task OnInitializedAsync()
		{
			var response = await Runtime.InvokeAsync<object>("JsWidgets.widget", new[] { "application/json", "for='htmlwidget-319103c47de8ed7a89f8'", @"{'x':{'word':['에너지','희토류','NDPR','MATERIALS','NAVER','플라스틱','친환경','바이오','연료전지','에너지원','CCUS','ESG','글로벌','농축액','배터리','수익성','풍력발전','PLA','생분해','석유화학','1분기','AREC','생산업체','시스템','HDRO','NDPRAGNET','PHA','THOTHFUND','이산화탄소','자동차','증권사','칼텍스','KBSTAR','NHN','SMDDACKA','가속화','네이버','롯데케미칼','무역전쟁','배출량','산화물','상업화','상용화','옥수수','온실가스','전세계','청정에너지','판매량','포장재','포트폴리오','프로젝트','환경규제'],'freq':[54,33,19,16,15,14,12,9,9,8,7,7,7,7,7,7,7,6,6,6,5,5,5,5,4,4,4,4,4,4,4,4,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3],'fontFamily':'Segoe UI','fontWeight':'bold','color':'random - dark','minSize':0,'weightFactor':3.33333333333333,'backgroundColor':'white','gridSize':0,'minRotation':-0.785398163397448,'maxRotation':0.785398163397448,'shuffle':true,'rotateRatio':0.4,'shape':'circle','ellipticity':0.65,'figBase64':null,'hover':null},'evals':[],'jsHooks':[]}" });
			response = await Runtime.InvokeAsync<object>("JsWidgets.widget", new[] { "application/htmlwidget-sizing", "for='htmlwidget-319103c47de8ed7a89f8'", @"{'viewer':{'width':450,'height':350,'padding':0,'fill':true},'browser':{'width':960,'height':500,'padding':0,'fill':true}}" });
		}
		[Inject]
		IJSRuntime Runtime
		{
			get; set;
		}
		*/
	}
}