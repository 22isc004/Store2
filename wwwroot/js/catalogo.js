// ===== CATÁLOGO - Filtros AJAX sin recargar página =====

document.addEventListener('DOMContentLoaded', function () {
    const filtrosForm = document.getElementById('filtrosForm');
    const productosContainer = document.getElementById('productosContainer');
    const ordenarSelect = document.getElementById('ordenarSelect');
    const btnAplicar = document.getElementById('aplicarFiltros');
    const btnLimpiar = document.getElementById('limpiarFiltros');
    const btnFiltrosMobile = document.getElementById('btnFiltrosMobile');
    const filtrosSidebar = document.getElementById('filtrosSidebar');

    if (!filtrosForm || !productosContainer) return;

    // Obtener los parámetros del formulario como objeto
    function obtenerParametros() {
        const params = new URLSearchParams();
        const datos = new FormData(filtrosForm);

        for (const [clave, valor] of datos.entries()) {
            if (valor) params.append(clave, valor);
        }

        // Agregar ordenamiento del select externo
        const ordenar = ordenarSelect?.value;
        if (ordenar) params.set('ordenar', ordenar);

        return params;
    }

    // Cargar productos via AJAX
    function cargarProductos(params) {
        productosContainer.classList.add('loading');
        productosContainer.style.opacity = '0.5';
        productosContainer.style.pointerEvents = 'none';

        const url = `/Catalogo?${params.toString()}`;

        fetch(url, {
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        })
        .then(r => r.text())
        .then(html => {
            productosContainer.innerHTML = html;
            productosContainer.style.opacity = '1';
            productosContainer.style.pointerEvents = 'auto';
            productosContainer.classList.remove('loading');

            // Re-registrar event listeners en los nuevos botones de agregar al carrito
            productosContainer.querySelectorAll('.btn-add-cart').forEach(btn => {
                btn.addEventListener('click', function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    agregarAlCarrito(this.dataset.id, 1, null, null);
                });
            });

            // Hacer scroll suave hacia los productos
            productosContainer.scrollIntoView({ behavior: 'smooth', block: 'start' });
        })
        .catch(err => {
            console.error('Error al cargar productos:', err);
            productosContainer.style.opacity = '1';
            productosContainer.style.pointerEvents = 'auto';
            mostrarNotificacion('Error al cargar productos', 'error');
        });
    }

    // Aplicar filtros con AJAX
    if (btnAplicar) {
        btnAplicar.addEventListener('click', function () {
            cargarProductos(obtenerParametros());
        });
    }

    // Limpiar todos los filtros
    if (btnLimpiar) {
        btnLimpiar.addEventListener('click', function () {
            filtrosForm.reset();
            cargarProductos(new URLSearchParams());
        });
    }

    // Cambiar ordenamiento en tiempo real
    if (ordenarSelect) {
        ordenarSelect.addEventListener('change', function () {
            cargarProductos(obtenerParametros());
        });
    }

    // Toggle filtros en mobile
    if (btnFiltrosMobile && filtrosSidebar) {
        btnFiltrosMobile.addEventListener('click', function () {
            filtrosSidebar.classList.toggle('visible');
        });
    }

    // Radios de categoría: aplicar al hacer clic directamente
    filtrosForm.querySelectorAll('input[name="categoriaId"]').forEach(radio => {
        radio.addEventListener('change', function () {
            cargarProductos(obtenerParametros());
        });
    });

    // Checkboxes de oferta/destacados: aplicar al hacer clic
    filtrosForm.querySelectorAll('input[type="checkbox"]').forEach(checkbox => {
        checkbox.addEventListener('change', function () {
            cargarProductos(obtenerParametros());
        });
    });

    // Marca: aplicar al seleccionar
    filtrosForm.querySelectorAll('input[name="marca"]').forEach(radio => {
        radio.addEventListener('change', function () {
            cargarProductos(obtenerParametros());
        });
    });
});
