<%@ Page Language="C#" %>
<%@ Import Namespace="BusinessRefinery.Barcode.Web" %>
<%
    HttpLinearBarcode.drawBarcode(Request, Response);
%>
