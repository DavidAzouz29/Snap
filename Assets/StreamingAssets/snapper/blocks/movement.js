
goog.require('Blockly.Blocks');
goog.require('Blockly');

Blockly.Blocks['transform_transform'] = {
  init: function() {
    this.appendValueInput("Move")
        .setCheck(null)
        .appendField("Transform");
    this.setPreviousStatement(true, null);
    this.setNextStatement(true, null);
    this.setColour(195);
 this.setTooltip("Move Character");
 this.setHelpUrl("");
  }
};

Blockly.Blocks['math_vector3'] = {
  init: function() {
    this.appendValueInput("v3")
        .setCheck(null)
        .appendField("Vector")
        .appendField(new Blockly.FieldNumber(0), "X")
        .appendField(new Blockly.FieldNumber(0), "Y")
        .appendField(new Blockly.FieldNumber(0), "Z");
    this.setOutput(true, null);
    this.setColour(210);
 this.setTooltip("Can be used for locomotion.");
 this.setHelpUrl("https://docs.unity3d.com/ScriptReference/Vector3.html");
  }
};

Blockly.Blocks['move_move'] = {
  init: function() {
    this.appendValueInput("Vector")
        .setCheck("Vector")
        .appendField("Move");
    this.setPreviousStatement(true, null);
    this.setNextStatement(true, null);
    this.setColour(210);
 this.setTooltip("");
 this.setHelpUrl("https://docs.unity3d.com/ScriptReference/Transform.html");
  }
};

Blockly.Blocks['move_facing'] = {
  init: function() {
    this.appendValueInput("directions")
        .setCheck(["Number", "Vector"])
        .appendField(new Blockly.FieldDropdown([["Vec Forward","Forward"], ["Vec Up","Up"], ["Vec Right","Right"]]), "direction");
    this.setOutput(true, "Vector");
    this.setColour(230);
 this.setTooltip("Select Direction for transform.");
 this.setHelpUrl("https://docs.unity3d.com/ScriptReference/Transform.html");
  }
};

Blockly.Blocks['move_speed'] = {
  init: function() {
    this.appendDummyInput()
        .appendField(new Blockly.FieldDropdown([["speed slow","slow"], ["speed medium","medium"], ["speed fast","fast"]]), "speed");
    this.setOutput(true, "Number");
    this.setColour(230);
 this.setTooltip("Speed at which we travel.");
 this.setHelpUrl("");
  }
};