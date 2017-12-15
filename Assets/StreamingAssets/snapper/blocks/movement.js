
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

Blockly.Blocks['math_arithmetic_operator'] = {
  init: function() {
    this.appendValueInput("A")
        .setCheck(null);
    this.appendDummyInput()
        .appendField(new Blockly.FieldDropdown([["+=","plusEqual"], ["-=","minusEqual"], ["*=","timesEqual"], ["/=","divideEqual"], ["%=","modulusEqual"]]), "OP");
    this.appendValueInput("B")
        .setCheck(null);
    this.setInputsInline(true);
    this.setOutput(true, null);
    this.setColour(230);
 this.setTooltip("");
 this.setHelpUrl("%{BKY_MATH_ARITHMETIC_HELPURL}");
  }
};

Blockly.Blocks['move_move'] = {
  init: function() {
    this.appendValueInput("Vector")
        .setCheck("Vector")
        .appendField("Move");
    this.appendDummyInput()
        .appendField(new Blockly.FieldDropdown([["speed slow","speedSlow"], ["speed medium","speedMedium"], ["speed fast","speedFast"]]), "speed");
    this.setInputsInline(true);
    this.setPreviousStatement(true, null);
    this.setNextStatement(true, null);
    this.setColour(210);
 this.setTooltip("");
 this.setHelpUrl("https://docs.unity3d.com/ScriptReference/Transform.html");
  }
};

Blockly.Blocks['move_rotate'] = {
  init: function() {
    this.appendValueInput("Vector")
        .setCheck("Vector")
        .appendField("Rotate")
        .appendField(new Blockly.FieldDropdown([["clockwise","clockwise"], ["anticlockwise","anticlockwise"]]), "orientation");
    this.appendDummyInput()
        .appendField(new Blockly.FieldDropdown([["speed slow","speedSlow"], ["speed medium","speedMedium"], ["speed fast","speedFast"]]), "speed");
    this.setInputsInline(true);
    this.setPreviousStatement(true, null);
    this.setNextStatement(true, null);
    this.setColour(210);
 this.setTooltip("");
 this.setHelpUrl("https://docs.unity3d.com/ScriptReference/Transform.Rotate.html");
  }
};

Blockly.Blocks['move_rotate_angle'] = {
  init: function() {
    this.appendValueInput("Vector")
        .setCheck("Vector")
        .appendField("Rotate")
        .appendField(new Blockly.FieldDropdown([["clockwise","clockwise"], ["anticlockwise","anticlockwise"]]), "orientation");
    this.appendDummyInput()
        .appendField("Angle")
        .appendField(new Blockly.FieldAngle(90), "angle");
    this.setInputsInline(true);
    this.setPreviousStatement(true, null);
    this.setNextStatement(true, null);
    this.setColour(210);
 this.setTooltip("");
 this.setHelpUrl("https://docs.unity3d.com/ScriptReference/Transform.Rotate.html");
  }
};

Blockly.Blocks['move_scale'] = {
  init: function() {
    this.appendDummyInput()
        .appendField(new Blockly.FieldDropdown([["Scale small","scaleSmall"], ["Scale medium","scaleMedium"], ["Scale big","scaleBig"]]), "scale");
    this.setInputsInline(true);
    this.setPreviousStatement(true, null);
    this.setNextStatement(true, null);
    this.setColour(210);
 this.setTooltip("");
 this.setHelpUrl("https://docs.unity3d.com/ScriptReference/Transform.Rotate.html");
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

Blockly.Blocks['move_direction'] = {
  init: function() {
    this.appendDummyInput()
        .appendField("Direction")
        .appendField(new Blockly.FieldDropdown([["clockwise","clockwise"], ["anticlockwise","anticlockwise"]]), "orientation")
        .appendField("angle")
        .appendField(new Blockly.FieldAngle(90), "angle");
    this.appendValueInput("Vector")
        .setCheck("Vector");
    this.setInputsInline(true);
    this.setPreviousStatement(true, null);
    this.setNextStatement(true, null);
    this.setColour(210);
 this.setTooltip("");
 this.setHelpUrl("https://docs.unity3d.com/ScriptReference/Quaternion.AngleAxis.html");
  }
};

Blockly.Blocks['move_jump'] = {
  init: function() {
    this.appendDummyInput()
        .appendField("Jump");
    this.appendValueInput("Dir")
        .setCheck("Vector")
        .appendField("Direction");
    this.appendValueInput("Speed")
        .setCheck(null)
        .appendField("Speed");
    this.setInputsInline(true);
    this.setPreviousStatement(true, null);
    this.setNextStatement(true, null);
    this.setColour(230);
 this.setTooltip("https://docs.unity3d.com/ScriptReference/Rigidbody.AddForce.html");
 this.setHelpUrl("");
  }
};
