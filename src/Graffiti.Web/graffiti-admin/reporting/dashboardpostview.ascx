<%@ Control Language="C#" %>

<h3 style="margin-bottom: -10px; margin-top: 20px;">Views in the last 30 days</h3>
<div id="linechart">
	<strong>Unable to display the chart</strong>
</div>

<script type="text/javascript">
// <[CDATA[
	var so = new SWFObject("reporting/amline.swf", "amline", "100%", "200", "8", "#ffffff");
	so.addVariable("path", "reporting/");
	so.addVariable("settings_file", escape("reporting/linegraph.xml"));
	so.addVariable("data_file", escape("reporting/charts.ashx?report=ViewsByDate&minDate=<asp:Literal id="
	minDate
	" runat="
	server
	"/>&maxDate=<asp:Literal id="maxDate" runat="server" />&fromDashboard=1"))
	;
	so.addVariable("preloader_color", "#999999");
	so.addParam('wmode', 'transparent');
	so.write("linechart");
// ]]>
</script>

<script runat="server">

	private void Page_Load(object sender, EventArgs e)
	{
		if (!Page.IsPostBack)
		{
			minDate.Text = DateTime.Now.AddDays(-30).Ticks.ToString();
			maxDate.Text = DateTime.Now.Ticks.ToString();
		}
	}

</script>