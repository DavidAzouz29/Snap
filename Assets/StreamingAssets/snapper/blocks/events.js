Blockly.Blocks['events_update'] = {
  init: function() {
    this.appendDummyInput()
        .appendField("Update");
    this.appendStatementInput("code")
        .setCheck(null);
    this.setColour(20);
 this.setTooltip("Called every frame.");
 this.setHelpUrl("");
  }
};

Blockly.Blocks['events_fixed_update'] = {
  init: function() {
    this.appendDummyInput()
        .appendField("Fixed Update");
    this.appendStatementInput("code")
        .setCheck(null);
    this.setColour(20);
 this.setTooltip("Called every fixed framerate frame. Used for Physics calculations.");
 this.setHelpUrl("https://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html");
  }
};

Blockly.Blocks['events_start'] = {
  init: function() {
    this.appendDummyInput()
        .appendField("Start");
    this.appendStatementInput("code")
        .setCheck(null);
    this.setColour(20);
 this.setTooltip("Used for Initialising.");
 this.setHelpUrl("");
  }
};

Blockly.Blocks['events_init'] = {
  init: function() {
    this.appendDummyInput()
        .appendField("Init");
    this.appendStatementInput("code")
        .setCheck(null);
    this.setColour(20);
 this.setTooltip("Used for variables.");
 this.setHelpUrl("");
  }
};

Blockly.Blocks['events_awake'] = {
  init: function() {
    this.appendDummyInput()
        .appendField("Awake");
    this.appendStatementInput("code")
        .setCheck(null);
    this.setColour(20);
 this.setTooltip("Used for Initialising.");
 this.setHelpUrl("");
  }
};

Blockly.Blocks['events_keypress'] = {
  init: function() {
    this.appendDummyInput()
        .appendField("When I Press Once")
        .appendField(new Blockly.FieldTextInput("W"), "KeyPress");
    this.appendStatementInput("code")
        .setCheck(null);
    this.setPreviousStatement(true, null);
    this.setNextStatement(true, null);
    this.setColour(20);
 this.setTooltip("KeyPress");
 this.setHelpUrl("https://docs.unity3d.com/ScriptReference/KeyCode.html");
  }
};

Blockly.Blocks['events_keyhold'] = {
  init: function() {
    this.appendDummyInput()
        .appendField("When I Hold")
        .appendField(new Blockly.FieldTextInput("W"), "KeyHold");
    this.appendStatementInput("code")
        .setCheck(null);
    this.setPreviousStatement(true, null);
    this.setNextStatement(true, null);
    this.setColour(20);
 this.setTooltip("KeyHold");
 this.setHelpUrl("https://docs.unity3d.com/ScriptReference/Input.html");
  }
};
