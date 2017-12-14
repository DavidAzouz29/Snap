'use strict';
var workspace = null;
var fakeDragStack = [];

function start() {
  setBackgroundColor();

  // Parse the URL arguments.
  var match = location.search.match(/dir=([^&]+)/);
  var rtl = match && match[1] == 'rtl';
  document.forms.options.elements.dir.selectedIndex = Number(rtl);

  var toolbox = getToolboxElement();
  document.forms.options.elements.toolbox.selectedIndex =
  Number(toolbox.getElementsByTagName('category').length == 0);
  match = location.search.match(/side=([^&]+)/);
  var side = match ? match[1] : 'start';
  document.forms.options.elements.side.value = side;
  // Create main workspace.
  workspace = Blockly.inject('blocklyDiv',
  { comments: true,
    collapse: true,
    disable: true,
    grid:
    {spacing: 25,
      length: 3,
      colour: '#ccc',
      snap: true},
      horizontalLayout: side == 'top' || side == 'bottom',
      maxBlocks: Infinity,
      media: 'https://blockly-demo.appspot.com/static/media/',
      oneBasedIndex: true,
      readOnly: false,
      rtl: rtl,
      scrollbars: true,
      toolbox: toolbox,
      toolboxPosition: side == 'top' || side == 'start' ? 'start' : 'end',
      zoom:
      {controls: true,
        wheel: true,
        startScale: 1.0,
        maxScale: 4,
        minScale: .25,
        scaleSpeed: 1.1}
      });

      // Restore previously displayed text.
      if (sessionStorage) {
        var text = sessionStorage.getItem('textarea');
        if (text) {
          document.getElementById('importExport').value = text;
        }
        // Restore event logging state.
        var state = sessionStorage.getItem('liveUpdates');
        liveUpdates(Boolean(Number(state)));
        var stateLog = sessionStorage.getItem('logEvents');
        logEvents(Boolean(Number(stateLog)));
      } else {
        // MSIE 11 does not support sessionStorage on file:// URLs.
        liveUpdates(true);
        logEvents(false);
      }
      taChange();
    }

    function setBackgroundColor() {
      var lilac = '#d6d6ff';

      var currentPage = window.location.href;
      var regexFile = /^file[\S]*$/;

      if (regexFile.test(currentPage)) {
        document.getElementsByTagName('body')[0].style.backgroundColor = lilac;
      }
    }

    function getToolboxElement() {
      var match = location.search.match(/toolbox=([^&]+)/);
      return document.getElementById('toolbox-' + (match ? match[1] : 'categories')); //'snapper'));
    }

    function toXml() {
      var output = document.getElementById('importExport');
      var xml = Blockly.Xml.workspaceToDom(workspace);
      output.value = Blockly.Xml.domToPrettyText(xml);
      output.focus();
      output.select();
      taChange();
    }

    function fromXml() {
      var input = document.getElementById('importExport');
      var xml = Blockly.Xml.textToDom(input.value);
      Blockly.Xml.domToWorkspace(xml, workspace);
      taChange();
    }

    function toCode(lang, fileExtention) {
      var output = document.getElementById('importExport');
      output.value = Blockly[lang].workspaceToCode(workspace);
      taChange();
    }

    // Disable the "Import from XML" button if the XML is invalid.
    // Preserve text between page reloads.
    function taChange() {
      var textarea = document.getElementById('importExport');
      if (sessionStorage) {
        sessionStorage.setItem('textarea', textarea.value);
      }
      var valid = true;
      try {
        Blockly.Xml.textToDom(textarea.value);
      } catch (e) {
        valid = false;
      }
      document.getElementById('import').disabled = !valid;
    }

    function liveUpdates(state) {
      var checkbox = document.getElementById('liveCheck');
      checkbox.checked = state;
      if (sessionStorage) {
        sessionStorage.setItem('logEvents', Number(state));
      }
      if (state) {
        workspace.addChangeListener(loggerTextarea);
      } else {
        workspace.removeChangeListener(loggerTextarea);
      }
    }

    function logEvents(stateLog) {
      var checkbox = document.getElementById('logCheck');
      checkbox.checked = stateLog;
      if (sessionStorage) {
        sessionStorage.setItem('logEvents', Number(stateLog));
      }
      if (stateLog) {
        workspace.addChangeListener(logger);
      } else {
        workspace.removeChangeListener(logger);
      }
    }

    function loggerTextarea(e) {
      toCode('JavaScript');
    }

    function logger(e) {
      console.log(e);
    }

  /*function airstrike(n) {
    var prototypes = [];
    var toolbox = getToolboxElement();
    var blocks = toolbox.getElementsByTagName('block');
    for (var i = 0, block; block = blocks[i]; i++) {
      prototypes.push(block.getAttribute('type'));
    }
    for (var i = 0; i < n; i++) {
      var prototype = prototypes[Math.floor(Math.random() * prototypes.length)];
      var block = workspace.newBlock(prototype);
      block.initSvg();
      block.getSvgRoot().setAttribute('transform', 'translate(' +
      Math.round(Math.random() * 450 + 40) + ', ' +
      Math.round(Math.random() * 600 + 40) + ')');
      block.render();
    }
  }

  function fakeDrag(id, dx, dy, opt_workspace) {
    var ws = opt_workspace || Blockly.getMainWorkspace();
    var blockToDrag = ws.getBlockById(id);

    if (!blockToDrag) {
      fakeDragWrapper();
      return;
    }
    var blockTop = blockToDrag.svgGroup_.getBoundingClientRect().top;
    var blockLeft = blockToDrag.svgGroup_.getBoundingClientRect().left;

    // Click somewhere on the block.
    var mouseDownEvent = new MouseEvent('mousedown',
    {clientX: blockLeft + 5, clientY: blockTop + 5});
    blockToDrag.onMouseDown_(mouseDownEvent);

    // Throw in a move for good measure.
    setTimeout(
      function() {
        var mouseMoveEvent = new MouseEvent('mousemove',
        {clientX: blockLeft + dx,
          clientY: blockTop + dy});
          blockToDrag.onMouseMove_(mouseMoveEvent);

          // Drop at dx, dy.
          setTimeout(
            function() {
              var mouseUpEvent = new MouseEvent('mouseup',
              {clientX: blockLeft + dx,
                clientY: blockTop + dy});
                blockToDrag.onMouseUp_(mouseUpEvent);

                setTimeout(fakeDragWrapper(), 100);
              }, 100);
            }, 100);
          };

          function fakeDragWrapper() {
            var dragInfo = fakeDragStack.pop();
            if (dragInfo) {
              fakeDrag(dragInfo.id, dragInfo.dx, dragInfo.dy, dragInfo.workspace);
            }
          }

          function fakeManyDrags() {
            var blockList = workspace.getAllBlocks();
            for (var i = 0; i < 2 * blockList.length; i++) {
              fakeDragStack.push(
                {
                  id: blockList[Math.round(Math.random() * (blockList.length - 1))].id,
                  // Move some blocks up and to the left, but mostly down and to the right.
                  dx: Math.round((Math.random() - 0.25) * 200),
                  dy: Math.round((Math.random() - 0.25) * 200),
                  workspace: workspace
                });
              }
              fakeDragWrapper();
            }

            function spaghetti(n) {
              var xml = spaghettiXml;
              for(var i = 0; i < n; i++) {
                xml = xml.replace(/(<(statement|next)( name="DO0")?>)<\//g,
                '$1' + spaghettiXml + '</');
              }
              xml = '<xml xmlns="http://www.w3.org/1999/xhtml">' + xml + '</xml>';
              var dom = Blockly.Xml.textToDom(xml);
              console.time('Spaghetti domToWorkspace');
              Blockly.Xml.domToWorkspace(dom, workspace);
              console.timeEnd('Spaghetti domToWorkspace');
            }
            var spaghettiXml = [
              '  <block type="controls_if">',
              '    <value name="IF0">',
              '      <block type="logic_compare">',
              '        <field name="OP">EQ</field>',
              '        <value name="A">',
              '          <block type="math_arithmetic">',
              '            <field name="OP">MULTIPLY</field>',
              '            <value name="A">',
              '              <block type="math_number">',
              '                <field name="NUM">6</field>',
              '              </block>',
              '            </value>',
              '            <value name="B">',
              '              <block type="math_number">',
              '                <field name="NUM">7</field>',
              '              </block>',
              '            </value>',
              '          </block>',
              '        </value>',
              '        <value name="B">',
              '          <block type="math_number">',
              '            <field name="NUM">42</field>',
              '          </block>',
              '        </value>',
              '      </block>',
              '    </value>',
              '    <statement name="DO0"></statement>',
              '    <next></next>',
              '  </block>'].join('\n');*/
