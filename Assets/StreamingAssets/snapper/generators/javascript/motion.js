Blockly.JavaScript['transform_transform'] = function(block) {
  var value_move = Blockly.JavaScript.valueToCode(block, 'Move', Blockly.JavaScript.ORDER_ATOMIC);
  // TODO: Assemble JavaScript into code variable.
  var code = '...;\n';
  return code;
};

Blockly.JavaScript['math_vector3'] = function(block) {
  var number_x = block.getFieldValue('X');
  var number_y = block.getFieldValue('Y');
  var number_z = block.getFieldValue('Z');
  var value_v3 = Blockly.JavaScript.valueToCode(block, 'v3', Blockly.JavaScript.ORDER_ATOMIC);
  // TODO: Assemble JavaScript into code variable.
  var code = '...';
  // TODO: Change ORDER_NONE to the correct strength.
  return [code, Blockly.JavaScript.ORDER_NONE];
};

Blockly.JavaScript['move_move'] = function(block) {
  var value_vector = Blockly.JavaScript.valueToCode(block, 'Vector', Blockly.JavaScript.ORDER_ATOMIC);
  // TODO: Assemble JavaScript into code variable.
  var code = '...;\n';
  return code;
};

Blockly.JavaScript['move_facing'] = function(block) {
  var dropdown_direction = block.getFieldValue('direction');
  var value_directions = Blockly.JavaScript.valueToCode(block, 'directions', Blockly.JavaScript.ORDER_ATOMIC);
  // TODO: Assemble JavaScript into code variable.
  var code = '...';
  // TODO: Change ORDER_NONE to the correct strength.
  return [code, Blockly.JavaScript.ORDER_NONE];
};

Blockly.JavaScript['move_speed'] = function(block) {
  var dropdown_speed = block.getFieldValue('speed');
  // TODO: Assemble JavaScript into code variable.
  var code = '...';
  // TODO: Change ORDER_NONE to the correct strength.
  return [code, Blockly.JavaScript.ORDER_NONE];
};