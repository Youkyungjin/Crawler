<%@ Page Language="C#" %>
<%@ Import Namespace="BusinessRefinery.Barcode.Web" %>
<%
    HttpQRCode.drawBarcode(Request, Response);
%>
