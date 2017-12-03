goog.require('Blockly.JavaScript');

Blockly.JavaScript['transform_transform'] = function(block) {
  var value_move = Blockly.JavaScript.valueToCode(block, 'Move', Blockly.JavaScript.ORDER_NEW);
  // TODO: Assemble JavaScript into code variable.
  var code = 'transform.position = ' + value_move + ';\n';
  return code;
};

Blockly.JavaScript['math_vector3'] = function(block) {
  var number_x = block.getFieldValue('X');
  var number_y = block.getFieldValue('Y');
  var number_z = block.getFieldValue('Z');
  var value_v3 = Blockly.JavaScript.valueToCode(block, 'v3', Blockly.JavaScript.ORDER_NEW);
  // TODO: Assemble JavaScript into code variable.
  //var code = 'new Vector3({0}, {1}, {2})'.format(number_x, number_y, number_z);
  var code = 'Vector3(' + number_x + ', ' + number_y + ', ' + number_z + ')'; //;\n
  // TODO: Change ORDER_NONE to the correct strength.
  return [code, Blockly.JavaScript.ORDER_ASSIGNMENT];
};

Blockly.JavaScript['move_move'] = function(block) {
  var value_vector = Blockly.JavaScript.valueToCode(block, 'Vector', Blockly.JavaScript.ORDER_NONE);
  // TODO: Assemble JavaScript into code variable.
  if(value_vector == null)
    value_vector = 'Vector3.forward'
  var code = 'transform.Translate(' + value_vector + ' * Time.deltaTime);\n';
  return code;
};

Blockly.JavaScript['move_facing'] = function(block) {
  var dropdown_direction = block.getFieldValue('direction');
  var value_directions = Blockly.JavaScript.valueToCode(block, 'directions', Blockly.JavaScript.ORDER_NONE);
  // TODO: Assemble JavaScript into code variable.
  var code = 'forward';
  switch (dropdown_direction) {
    case 'Forward':
      code = 'forward';
      break;
      case 'Up':
        code = 'up';
        break;
        case 'Right':
          code = 'right';
          break;
      break;
    default:
      code = 'forward';
      break;
  }
  code = 'Vector3.' + code;
  // TODO: Change ORDER_NONE to the correct strength.
  return [code, Blockly.JavaScript.ORDER_NONE];
};

Blockly.JavaScript['move_speed'] = function(block) {
  var dropdown_speed = block.getFieldValue('speed');
  // TODO: Assemble JavaScript into code variable.
  var code = '0f';
  switch (dropdown_speed) {
    case 'slow':
      code = '2f';
      break;
      case 'medium':
        code = '5f';
        break;
        case 'fast':
          code = '10f';
          break;
    default:
    code = '2f';
  }
  // TODO: Change ORDER_NONE to the correct strength.
  return [code, Blockly.JavaScript.ORDER_NONE];
};
