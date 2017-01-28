<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Email" Id="CR306015" OpenAs="Form" Type="SimpleScreen" Visible="false" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <sm:Container Name="Message">
    <sm:Field Name="FromMailAccountID" />
    <sm:Field Name="To" />
    <sm:Field Name="CC" />
    <sm:Field Name="BCC" />
    <sm:Field Name="Subject" />
    <sm:Field Name="ActivityDetails" TextType="HTML" Container="Message_"/>
    <sm:Group Collapsable="true" Collapsed="true" DisplayName="Details">
      <sm:Field Container="Details" Name="Date"/>
      <sm:Field Container="Details" DisplayName="Time" Name="StartTime"/>
      <sm:Field Container="Details" Name="Incoming" ForceIsDisabled="true"/>
      <sm:Field Container="Details" Name="Internal"/>
      <sm:Field Container="Details" Name="Owner"/>
      <sm:Field Container="Details" Name="RelatedEntityDescription"/>
      <sm:Field Container="Details" Name="Parent"/>
      <sm:Field Container="Details" Name="MailStatus"/>
    </sm:Group>
    <sm:Action Behavior="Save" Context="Record" Name="Save"/>
    <sm:Action Behavior="Cancel" Context="Record" Name="Cancel"/>
    <sm:Action Behavior="Record" Context="Record" Name="Send"/>
    <sm:Action Behavior="Record" Context="Record" Name="ReplyAllAction"/>
    <sm:Action Behavior="Record" Context="Record" Name="ForwardAction"/>
  </sm:Container>
</sm:Screen>

