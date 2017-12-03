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