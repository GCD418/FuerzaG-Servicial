// Configuración por defecto
const defaultDataTableConfig = {
    pageLength: 15,
    language: {
        lengthMenu: 'Registros por página: _MENU_',
        zeroRecords: 'No se encontraron resultados',
        info: 'Mostrando _START_ a _END_ de _TOTAL_ registros',
        infoEmpty: 'Mostrando 0 a 0 de 0 registros',
        infoFiltered: '(filtrados de _MAX_ registros totales)',
        search: '',
        searchPlaceholder: 'Buscar...',
        paginate: {
            first: 'Primero',
            previous: 'Anterior',
            next: 'Siguiente',
            last: 'Último'
        }
    },
    ordering: true,
    dom: '<"row align-items-center mb-3"' + 
         '<"col-12 col-sm-6"l>' + 
         '<"col-12 col-sm-6"f>>' + 
         '<"row"<"col-sm-12"tr>>' + 
         '<"row align-items-center mt-3"' + 
         '<"col-12 col-sm-5"i>' + 
         '<"col-12 col-sm-7"p>>',
    classes: {
        sLengthSelect: 'form-select form-select-sm',
        sFilterInput: 'form-control form-control-sm',
        wrapper: 'dataTables_wrapper dt-bootstrap5'
    }
};

/**
 * Inicializa una DataTable con configuración personalizada
 * @param {string} selector - Selector de la tabla (ej: '#productTable')
 * @param {object} customConfig - Configuración personalizada (opcional)
 * @returns {object} Instancia de DataTable
 */
function initDataTable(selector, customConfig = {}) {
    // Combinar configuración por defecto con la personalizada
    const config = $.extend(true, {}, defaultDataTableConfig, customConfig);
    
    // Inicializar DataTable
    const table = $(selector).DataTable(config);
    
    // Personalizar el input de búsqueda
    customizeSearchInput();
    
    return table;
}

/**
 * Personaliza el input de búsqueda agregando un icono
 */
function customizeSearchInput() {
    const searchInput = $('.dataTables_filter input');
    
    if (searchInput.length && !searchInput.parent().hasClass('input-group')) {
        searchInput.unwrap().wrap('<div class="input-group input-group-sm"></div>');
        searchInput.before('<span class="input-group-text"><i class="fas fa-search"></i></span>');
        searchInput.addClass('form-control');
    }
}

/**
 * Inicializa múltiples DataTables desde un array de configuraciones
 * @param {array} configs - Array de objetos con selector y config
 * @returns {array} Array de instancias de DataTable
 */
function initMultipleDataTables(configs) {
    return configs.map(item => {
        return initDataTable(item.selector, item.config || {});
    });
}

// Exportar funciones para uso global
window.DataTableHelper = {
    init: initDataTable,
    initMultiple: initMultipleDataTables,
    defaultConfig: defaultDataTableConfig
};

// Auto-inicialización
$(document).ready(function() {
    $('[data-datatable]').each(function() {
        const $table = $(this);
        const customConfig = $table.data('datatable-config') || {};
        initDataTable('#' + $table.attr('id'), customConfig);
    });
});