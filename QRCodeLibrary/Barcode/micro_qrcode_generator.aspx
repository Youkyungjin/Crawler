<%@ Page Language="C#" %>
<%@ Import Namespace="BusinessRefinery.Barcode.Web" %>
<%
    HttpMicroQRCode.drawBarcode(Request, Response);
%>

