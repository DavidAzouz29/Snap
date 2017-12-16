Blockly.Blocks['physics_oncollisionenter'] = {
  init: function() {
    this.appendDummyInput()
        .appendField("OnCollisionEnter");
    this.appendStatementInput("code")
        .setCheck(null);
    this.setColour(90);
 this.setTooltip("Used to detect when something has collided.");
 this.setHelpUrl("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnCollisionEnter.html");
  }
};
